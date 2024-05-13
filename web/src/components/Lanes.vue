<template>
  <Loader v-if="loading"/>
  <div v-else class="modal-content-container lanes ">
    <div class="information">Banen met een randje eromheen zijn kind vriendelijk en bevatten elektronische hekjes</div>
    <div class="selectLaneContainer">

<!--      Lane times-->
      <table class="modalTable" >
        <tr>
          <th>
            <div style="width: 3rem; height: 3rem; display: flex; ">
              Start tijd
            </div>
          </th>
        </tr>
        <tr v-for="time in businessHours" class="modalTr">
          <td class="modalTd">{{time}}</td>
        </tr>
      </table>

<!--      Lanes boxes-->
      <table class="modalTable" v-for="lane in totalLanes" :class="{'kidLane' : lane < 3}">
        <tr>
          <th>
            <div class="bowlingbalImg">{{lane}}</div>
          </th>
        </tr>
        <tr class="modalTr"  v-for="timeframe in businessHours">
          <td class="modalTd">
            <div :class="[`Box${lane}${timeframe}`, { 'magicBowlen': (timeframe == 22 || timeframe == 23) }]" @click="selectLane(`Box${lane}${timeframe}`)" class="avaiablelity"></div>
          </td>
        </tr>
      </table>
    </div>

<!--    Buttons-->
    <div v-if="edit" class="maakReserveringBtn" @click="this.$refs.modal.toggle() as any">bewerk reservering</div>
    <div v-else="edit" class="maakReserveringBtn" @click="emitSelectedLanes()">Selecteer banen</div>
  </div>
</template>
<script lang="ts">
import Loader from "@/components/Loader.vue";
import {defineComponent} from "vue";
import type {LaneReservationTime, Reservation, ReservationLane} from "@/store/types";
import {roleMatch} from "@/store/auth-functions";
import {Roles} from "@/enums/Roles";
import {ReservationClient, UserClient} from "../../../ApiClient";
export default defineComponent({
  components:{Loader},
  props: {
    edit: Boolean,
    reservation: {},
    date: Date,
  },
  data() {
    return{
      reservation: {
        reservationDate: new Date(),
        adults: 0,
        children: 0,
        isCanceled: false,
        lanes: [] as ReservationLane[],
      } as Reservation,
      businessHours:[] as number[],
      totalLanes: 8,
      edit: false,
      loading: false,
      date: new Date(),
    }
  },

  methods:{
    selectLane(reference:string){
      //makes variables for easier use and less repetition
      let laneNumber = Number(reference.slice(3, -2))
      let timeNumber = Number(reference.slice(4))
      let lane = this.reservation.lanes.find(lane => lane.laneNumber == laneNumber)
      if (this.reservation){
        lane = this.reservation.lanes.find(lane => lane.laneNumber == laneNumber)
      }
      let element = document.querySelector(`.${reference}`)!
      //Check if lane is occupied
      if (this.checkIfLaneIsOccupied(element)){
        return
      }
      //checks if lane already exists
      if (lane){

        //checks if timeframe is already selected
        if (lane.timeframe.find((timeframe) => timeframe.time == timeNumber)){
          this.deselectLane(element, laneNumber, timeNumber, lane)
          return
        }
        if (!this.checkIfTimeframesAreConnected(laneNumber, timeNumber, lane)){
          return;
        }
        //checks if the maximum number of timeframes is reached, if the timeframes are connected to each other and if the user is an employee
        if (this.checkNumberOfTimeframes(laneNumber, lane) && roleMatch([Roles.CUSTOMER])){
          return;
        }

        //add timeNumber to lane
        let timeFrameObject: LaneReservationTime = {
          time: timeNumber,
        }
        lane.timeframe.push(timeFrameObject)

      } else {
        if (this.checkIfLimitOfLanesIsReached()){
          return
        }
        this.createLane(laneNumber, timeNumber)
      }
      element.classList.add('selected')
    },

    deselectLane(element:any, laneNumber:number, timeNumber:number, lane:ReservationLane){
      if (lane){
        //removes timeframe from timeframe array
        let index = lane.timeframe.findIndex((timeframe) => timeframe.time == timeNumber)
        lane.timeframe.splice(index, 1)

        this.checkIfTimeframeIsEmpty(laneNumber, timeNumber, lane)

        //removes selected class from element
        element.classList.remove('selected')
      }
    },

    //checks if timeframe is empty and deletes lane from selectedLanes
    checkIfTimeframeIsEmpty(laneNumber:number, timeNumber:number, lane:ReservationLane){
      //checks if timeframe is empty
      if (lane.timeframe.length == 0){
        //finds index of lane in reservation.lanes
        let index = this.reservation.lanes.findIndex((lane) => lane.laneNumber == laneNumber)
        //removes object from reservation.lanes
        this.reservation.lanes.splice(index, 1)
        this.$emit('resetCapacity')
      }
    },

    checkIfLimitOfLanesIsReached(){
      let lengthOfObject = Object.keys(this.reservation.lanes).length
      if(roleMatch([Roles.CUSTOMER]) && (lengthOfObject == 2)){
        this.$toast.info(`Je kan maar 2 banen selecteren`)
        return true;
      }
      else if(lengthOfObject == 8 && !roleMatch([Roles.CUSTOMER])){
        this.$toast.info(`Je kan maar 8 banen selecteren`)
        return true;
      }
      return false
    },

    //check if number of timeframes does not exceed limit (2)
    checkNumberOfTimeframes(laneNumber:number, lane:ReservationLane){

      if (roleMatch([Roles.CUSTOMER])) {
        //checks how many timeframes they have selected
        if (Object.keys(lane.timeframe).length == 2) {
          this.$toast.info("Je kan niet meer dan 2 uren reserveren")
          return true;
        }
      }
      return false
    },

    //checks if timeframes are connected to each other
    checkIfTimeframesAreConnected(laneNumber:number, timeNumber:number, lane:ReservationLane){
      //checks if time is connected to each other
      let validTimeFrames = []
      // let timeframeArray = []
      validTimeFrames.push((Math.min(...lane.timeframe.map(timeframe=> timeframe.time)) - 1))
      validTimeFrames.push((Math.max(...lane.timeframe.map(timeframe=> timeframe.time)) + 1))
      if (!validTimeFrames.includes(timeNumber)){
        this.$toast.info("Je moet een aansluitende tijd selecteren")
        return false;
      }
      return true
    },

    createLane(laneNumber:number, timeNumber:number){
      //if lane doesnt exist it makes it exist
      let timeFrameObject: LaneReservationTime = {
        time: timeNumber,
      }
      let laneObject: ReservationLane = {
        laneNumber: laneNumber,
        timeframe: [timeFrameObject]
      }
      this.reservation.lanes.push(laneObject)
    },

    //checks which lane is occupied and makes them occupied
    async checkAvailability(){
      //gets all occupied lanes at given date
      let lanes = await this.getOccupiedLanes()
      //loops through occupiedLanes
      Object.keys(lanes).forEach((lane,index) =>{
        let laneNumber = lanes[index].laneNumber
        //loops through timeframe
        for (let timeframe of lanes[index].timeframe){
          let element = document.querySelector(`.Box${laneNumber}${timeframe.time}`)!
          element.classList.add('bezet')
        }
      })
      if (this.$props.reservation != undefined){
        this.removeSelectlanes()
      }
    },

    //Removes selected lanes from occupied lanes
    removeSelectlanes(){
      Object.keys(this.reservation.lanes).forEach((lane,index) =>{
        let laneNumber = this.reservation.lanes[index].laneNumber
        //loops through timeframe
        for (let timeframe of this.reservation.lanes[index].timeframe){
          let element = document.querySelector(`.Box${laneNumber}${timeframe.time}`)!
          element.classList.remove('bezet')
        }
      })
    },

    //gets occupied lanes at given date
    async getOccupiedLanes(){
      this.loading = true

      //@ts-ignore
      let reservationClient = new ReservationClient(null as any, this.http)
      let response = await reservationClient.getOccupiedLanes(this.date)
      this.loading = false
      return response
    },

    async selectChosenLanes(){
      if (this.reservation.lanes.length != 0){
        Object.keys(this.reservation.lanes).forEach((lane,index) =>{
          let laneNumber = this.reservation.lanes[index].laneNumber
          //loops through timeframe
          for (let timeframe of this.reservation.lanes[index].timeframe){
            let element = document.querySelector(`.Box${laneNumber}${timeframe.time}`)!
            element.classList.add('selected')
          }
        })
      }
    },

    //checks if selectedLane is occupied
    checkIfLaneIsOccupied(element:any){
      if (element.classList.contains('bezet')) {
        //toast
        this.$toast.info("Deze baan is op dit tijdstip bezet")
        return true
      }
      return false
    },

    //checks if it is weekend for businesshours
    checkIfWeekend(){
      let date = new Date(this.date)
      console.log(date.getDay())
      if (date.getDay() == 0 || date.getDay() == 6){
        this.businessHours = [14,15,16,17,18,19,20,21,22,23]
        return
      }
      this.businessHours = [14,15,16,17,18,19,20,21]
    },

    emitSelectedLanes(){
      this.$emit('selectedLanes', this.reservation.lanes)
    },
  },
  mounted() {
    this.date = this.$props.date as Date
    this.reservation = this.$props.reservation as Reservation
    this.checkIfWeekend()

    this.$nextTick(() => {
      this.checkAvailability()
      this.selectChosenLanes()
      this.loading = false
    })
  },
})
</script>


<style scoped lang="scss">
@import "../assets/css/colors";

.disable{
  display: none;
}

.hero-booking-container{
  width: 900px;
  height: 90px;
  margin-bottom: 10rem;
  border-radius: 28px;
  background: $creme;
  display: flex;
  justify-content: space-evenly;
  align-items: center;
  //overflow: auto;
}

.field{
  display: flex;
  height: 50%;
  width: 15%;
  background: $orange-4;
  justify-content: space-between;
  cursor: pointer;

  &-content{
    display: flex;
    padding-left: 1rem;
    align-items: center;
    width: 70%;
    img {
      height: 50%;
      color: $orange-5;
      margin-right: .5rem;
    }
    p{
      color: $creme;
    }
  }

  &-action{
    width: 50px;
    background: $orange-5;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;

    img{
      width: 50%;
      filter: invert(1);
    }
  }
}
.dateField{
  width:25%;

  .field-content{
    background: $orange-4;
  }
}
.dp__theme_light {
  --dp-background-color: $orange-4;
  --dp-text-color: #212121;
  --dp-hover-color: $orange-4;
  --dp-hover-text-color: #212121;
  --dp-hover-icon-color: #959595;
  --dp-primary-color: #1976d2;
  --dp-primary-disabled-color: #6bacea;
  --dp-primary-text-color: #f8f5f5;
  --dp-secondary-color: #c0c4cc;
  --dp-border-color: $orange-4;
  --dp-menu-border-color: #ddd;
  --dp-border-color-hover: #aaaeb7;
  --dp-disabled-color: #f6f6f6;
  --dp-scroll-bar-background: #f3f3f3;
  --dp-scroll-bar-color: #959595;
  --dp-success-color: #76d275;
  --dp-success-color-disabled: #a3d9b1;
  --dp-icon-color: #959595;
  --dp-danger-color: #ff6f60;
  --dp-marker-color: #ff6f60;
  --dp-tooltip-color: #fafafa;
  --dp-disabled-color-text: #8e8e8e;
  --dp-highlight-color: rgb(25 118 210 / 10%);
  --dp-range-between-dates-background-color: var(--dp-hover-color, #f3f3f3);
  --dp-range-between-dates-text-color: var(--dp-hover-text-color, #212121);
  --dp-range-between-border-color: var(--dp-hover-color, #f3f3f3);
}
.reserverenBtn{
  background: $dark;
  width: 25%;

  .field-action{
    background: $gray-2;
    transition: transform 0.2s ease-in;
    cursor: pointer;

    img{
      filter: invert(1);
    }
  }
}
.banen{
  width: 15%;
}
.dropdown-content{
  padding: .5rem 1rem 0 1rem;
  height: 70%;
  width: 50%;
  z-index: 1000;

  .dropdown-field{
    width: 100%;
    height: 50%;
    display: flex;
    flex-direction: column;
    justify-content: space-around;

    &-title{
      margin: 0;
    }

    &-actions{
      width: 80%;
      height: 50%;
      //background: yellow;
      display: flex;
      flex-direction: row;
      background: $orange-4;

      .increase, .decrease{
        background: $orange-5;
        width: 25%;
        display: flex;
        justify-content: center;
        align-items: center;
        img{
          width: 80%;
          filter: invert(1);
        }
      }
      .decrease{
        img{
          transform: rotate(180deg);
        }
      }

      .value{
        width: 50%;
        display: flex;
        justify-content: center;
        align-items: center;
        input{
          width: 100%;
          height: 80%;
          //color: $creme;
          margin: 0;
        }
      }
    }
  }
}
.modal-content-container{
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  .information{
    width: 80%;
    margin: 0 auto;
    text-align: center;
  }
  .selectLaneContainer{
    margin-top: 2rem;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: row;
  }
  div {

    .modalTable {
      margin-right: 1rem;

      tr {
        height: 3rem;
        width: 3rem;
        display: flex;
        justify-content: center;
        align-items: center;

        td {
          width: 100%;
          height: 100%;
          display: flex;
          justify-content: center;
          align-items: center;

          .avaiablelity {
            width: 80%;
            height: 80%;
            background-color: $green;
            border-radius: 10px;
          }

          .selected {
            border: 2px solid $dark;
            background-image: url("@/assets/img/icon/check-mark-svgrepo-com.svg") !important;
            background-repeat: no-repeat;
            background-size: cover;
          }

          .magicBowlen {
            background-color: $disco;
          }
        }
      }


      .bowlingbalImg {
        color: $creme;
        transform: translateY(-1rem);
        width: 3rem;
        height: 3rem;
        display: flex;
        justify-content: center;
        align-items: center;
        background-image: url("@/assets/img/bowlinbal.png");
        background-repeat: no-repeat;
        background-size: cover;
        background-position: center;
      }
    }
  }
}
.kidLane {
  border: 2px solid silver;
  border-radius: 10px;
}
.maakReserveringBtn{
  margin-top: 2rem;
  max-width: 300px;
  height: auto;
  padding: .8rem;
  border-radius: 10px;
  background: $dark;
  color: $creme;
  cursor: pointer;
}
.notActive{
  display: none;
}

.confirmation{
  justify-content: left;
  align-items: center;
  p{
    text-align: left;
    justify-content: left;
  }

  .selectedLanesAndTimes{
    display: flex;
    align-items: center;
    flex-direction: row;
    margin-bottom: .3rem;
    .laneNumber{
      font-size: 1.5rem;
      font-weight: bold;
      color: $creme;
      width: 3rem;
      height: 3rem;
      margin-right: .5rem;
      display: flex;
      justify-content: center;
      align-items: center;
      background-image: url("@/assets/img/bowlinbal.png");
      background-repeat: no-repeat;
      background-size: cover;
      background-position: center;
    }
  }
  .confirmationButtons{
    display: flex;
    justify-content: center;

    div:first-child{
      margin-right: .5rem;
    }
    div:last-child{
      color: $dark;
      background: none;
      border: 2px solid $dark;
    }
  }
}
.colorBlack{
  color: black;
}

.bezet{
  background: red !important;
}
.dropdown{
  z-index: 999 !important;
}
.selecteerGebruikerBtn{
  text-align: center;
}
.selectUserTitle{
  display: flex;
  flex-direction: row;
  width: 300px;
  align-items: center;
}
.returnBtn{
  width: 20px;
  height: 20px;
  margin-right: 1rem;
  transform: rotateY(180deg);
  cursor: pointer;
}
.searchBar{
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
}
.searchUser{
  padding: .7rem;
  border-radius: .5rem;
  height: 35%;
  width: 60%;
  margin: 1rem;
}
.searchBtn{
  margin: 0;
  width: 30%;
  text-align: center;
}
.gebruikers{
  display: flex;
  flex-direction: column;
  //justify-content: center;
  align-items: center;
  overflow-y: auto;
  height: 60%;
  width: 300px;
}
.gebruiker{
  width: 260px;
  height: 60px;
  background: $orange-4;
  display: flex;
  flex-direction: row;
  //justify-content: center;
  align-items: center;
  margin-bottom: 1rem;
  padding: 0 1rem 0 1rem;
  border-radius: .5rem;
  cursor: pointer;
  .userCircle{
    width: 40px;
    height: 40px;
    //background: r;
    background: $gray-3;
    margin-right: 1rem;
    border-radius: 50%;
    justify-content: center;
    align-items: center;
  }
  div{
    display: flex;
    justify-content: center;
    flex-direction: column;
  }
  div:last-child{
    height: 100%;
  }
  p{
    margin: 0;
  }
}
.inputField {
  background-color: $creme;
  width: 30%;
  height: 55px;
  border-radius: 15px;
  margin-bottom: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
  outline: solid $dark 2px;
}
.inputField > input {
  padding: 1rem;
  border: none;
  background-color: transparent;
  width: 100%;
  height: 90%;
  font-size: 20px;
  font-weight: bold;
  //border: 2px solid $dark;
  color: $dark;
}
.inputField > input:focus {
  outline: none;
}

@media screen and (width < 920px){
  .hero-booking-container{
    flex-direction: column;
    height: 370px;
    margin-bottom: 1% !important;
    width: 40%;
    justify-content: normal;
  }
  .field{
    height: 40px;
    width: 90% !important;
    margin-top: 5%;
  }

}
@media only screen and (max-width: 800px) {
  .hero-booking-container{
    width: 300px;
    height: 320px!important;
  }
  .field{
    height: 40px;
    width: 260px !important;
    margin-top: 5%;
  }
  .disable {
    display: block;
  }
  .information{
    margin-bottom: 0;
    transform: translateY(40%);
  }
  .maakReserveringBtn{
    margin: 1rem 0 0 0 ;
  }
  .gebruikers{
    width: 90% !important;
  }
  .inputField {
    width: 90% !important;
  }
}
@media only screen and (max-width: 630px) {
  .modalTable{
    margin-right: .1rem !important;
  }
}
@media only screen and (max-width: 630px) {
  .selectLaneContainer{
    transform: scale(0.8) ;
  }
}
@media only screen and (max-width: 410px) {
  .selectLaneContainer{
    transform: scale(0.7) ;
  }

  .confirmationButtons{
    flex-direction: column !important;
  }
}


</style>

