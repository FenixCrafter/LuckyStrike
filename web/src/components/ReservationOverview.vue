<template>
  <div v-if="!selectUser && !loading" class="modal-content-container confirmation">

  <!--    Title-->
    <div><h2>Reservatie:</h2></div>

  <!--    Capacity-->
    <div><p>Aantal volwassenen: {{reservation.adults}}</p></div>
    <div><p>Aantal kinderen: {{reservation.children}}</p></div>

  <!--    Selected Lanes-->
    <div>
      <p>Geselecteerde Banen: </p>
      <div class="selectedLanesAndTimes" v-for="lane in reservation.lanes" :class="{'kidLane': lane.laneNumber <= 2}">
        <div class="laneNumber">{{lane.laneNumber}}</div>
        <p>{{getSelectedLaneTimeframe(lane.timeframe)}}</p>
      </div>
    </div>

  <!--    Costs-->
    <div><p>Kosten: €{{calculateReservationCosts()}}</p></div>

  <!--    Selected User-->
    <div v-if="!roleMatch([Roles.CUSTOMER]) && !edit">
      <p>Gebruiker:</p>

    <!--      Selected User-->
      <div v-if='selectedUser.fullName != "" || selectedUser.email != "" ' class="selectedGebruiker">
        <div class="gebruiker">
          <div class="userCircle">{{selectedUser.fullName.charAt(0)}}</div>
          <div>
            <p v-if="selectedUser.fullName.length <= 23">{{selectedUser.fullName}}</p>
            <p v-else>{{selectedUser.fullName.substring(0,18)}}...</p>
            <p v-if="selectedUser.email.length <= 23">{{selectedUser.email}}</p>
            <p v-else>{{selectedUser.email.substring(0,18)}}...</p>
          </div>
        </div>
      </div>

      <!--      No user selected-->
      <div v-else class="selectedGebruiker">
        <p>Geen gebruiker geselecteerd</p>
      </div>
    </div>
    <div v-if="!roleMatch([Roles.CUSTOMER]) && !edit">
      <div @click="$emit('selectUser')" class="maakReserveringBtn selecteerGebruikerBtn">Selecteer gebruiker</div>
    </div>

  <!--    Buttons-->
    <div class="confirmationButtons">
      <div v-if="!edit && roleMatch([Roles.CUSTOMER])" class="maakReserveringBtn" @click="$emit('confirmReservation')">Bevestig reservering</div>
      <div v-if="edit" class="maakReserveringBtn" @click="$emit('finishEditReservation')">Bewerk Reservering</div>
      <div v-if="!edit && !roleMatch([Roles.CUSTOMER])" class="maakReserveringBtn" @click="$emit('confirmReservationEmployee')">Bevestig reservering</div>
      <br class="disable">
      <div v-if="!edit" class="maakReserveringBtn" @click="$emit('cancelReservation')">annuleer reservering</div>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import {roleMatch} from "@/store/auth-functions";
import {Roles} from "@/enums/Roles";
import type { Reservation, ReservationLane, LaneReservationTime, User} from "@/store/types";
import {ReservationClient, UserClient} from "../../../ApiClient";
import Loader from "@/components/Loader.vue";
import SelectUser from "@/components/SelectUser.vue";
export default defineComponent({
  components: {SelectUser, Loader},
  computed: {
    Roles() {
      return Roles
    }
  },
  props: {
    reservationProp: {},
    selectedUserProp: {},
    edit: Boolean,
    date: Date,
  },
  data() {
    return{
      date: new Date(),
      reservation: {} as Reservation,
      selectUser: false,
      selectedUser:{
        fullName: "",
        email: "",
      } as User,
      loading:true
    }
  },
  methods:{
    //calculates reservation costs
    calculateReservationCosts(){
      let day = this.date.getDay()
      let totalCost = 0
      //checks if day is monday, tuesday, wednesday or thursday
      if (day >= 1 && day <= 4){
        //loops through selected Lanes
        this.reservation.lanes.forEach((lane) =>{
          //because price is the same no matter the hour of these days,
          //the length of the timeframe can be multiplied by 24 euros
          totalCost += (lane.timeframe.length * 24)
        })
      } else {
        //loops through selected Lanes
        Object.keys(this.reservation.lanes).forEach((lane) =>{
          //loops through timeframe and adds costs based on time of timeframe
          for (let timeframe of this.reservation.lanes[Number(lane)].timeframe){
            if (timeframe.time <= 17){
              totalCost += 28
            } else {
              totalCost += 33.50
            }
          }
        })
      }
      return totalCost
    },

    //gets lane timeframe for example if 14 is selected, timeframe is 14:00 to 15:00
    getSelectedLaneTimeframe(timeframeArray:LaneReservationTime[]){
      if (timeframeArray.length == 1){
        return `${timeframeArray[0].time}:00 - ${timeframeArray[0].time+1}:00`
      } else {
        return `${Math.min(...timeframeArray.map(timeframe => timeframe.time))}:00 - ${Math.max(...timeframeArray.map(timeframe => timeframe.time))+1}:00`
      }
    },

    roleMatch
  },
  mounted() {
    //@ts-ignore
    this.date = this.$props.date
    //@ts-ignore
    this.reservation = this.$props.reservationProp
    this.selectedUser = this.$props.selectedUserProp

    this.$nextTick(() => {
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
    margin-bottom: 1rem;
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

