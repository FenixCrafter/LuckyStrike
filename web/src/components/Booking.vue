<template>
  <Loader v-if="loading"/>
  <div ref="container" class="hero-booking-container">
<!--    Datepicker-->
    <div class="dateField field">
      <div class="field-content">
        <vue-date-picker :min-date="minDate" class="colorBlack" ref="datePicker" v-model="date" :enable-time-picker="false">{{date}}</vue-date-picker>
      </div>
      <div class="field-action" @click="function(){
          let datePicker = $refs.datePicker as any
            capacityDropdown = false
          datePicker.openMenu()}">
        <img src="@/assets/img/icon/arrow-down.svg" alt="">
      </div>
    </div>

<!--    Select Lanes-->
    <div @click="openModal(false)" class="banen field">
      <div class="field-content">
        <img src="@/assets/img/icon/bowling-ball-svgrepo-com.svg" alt="">
        <p style="color: black; text-align: center">{{ reservation.lanes.length }}</p>
      </div>
      <div class="field-action">
        <img src="@/assets/img/icon/arrow-down.svg" alt="">
      </div>
    </div>

<!--    Capacity-->
    <div  class="capacityField field">
      <div @click="handleCapacityDropdownChange(true)" class="field-content">
        <img src="@/assets/img/icon/users.svg" alt="">
        <p style="color: black">{{reservation.adults + reservation.children}}</p>
      </div>
      <div @click="handleCapacityDropdownChange(true)" class="field-action">
        <img src="@/assets/img/icon/arrow-down.svg" alt="">
      </div>
      <div style="position:relative; top: 120%;
  left: -100%;" v-if="capacityDropdown">
        <dropdown-component class="dropdown" @close="handleCapacityDropdownChange(false)"
                            title="Personen" pos-y="5%" pos-x="0" height="20vh" width="10vh">
          <div class="dropdown-content">
            <div  class="dropdown-field adult">
              <p class="dropdown-field-title">Volwassenen</p>
              <div class="dropdown-field-actions">
                <div class="decrease" @click="decreaseAdult">
                  <img src="@/assets/img/icon/arrow-right.svg" alt="">
                </div>
                <div class="value">
                  <input @change="checkIfAdultIsValid" v-model="reservation.adults" type="number" name="adult" id="adult">
                </div>
                <div class="increase" @click="increaseAdult">
                  <img src="@/assets/img/icon/arrow-right.svg" alt="">
                </div>
              </div>
            </div>
            <div  class="dropdown-field children">
              <p class="dropdown-field-title">Kinderen</p>
              <div class="dropdown-field-actions">
                <div class="decrease" @click="decreaseChild">
                  <img src="@/assets/img/icon/arrow-right.svg" alt="">
                </div>
                <div class="value">
                  <input @change="checkIfChildIsValid" v-model="reservation.children" type="number" name="child" id="child">

                </div>
                <div class="increase" @click="increaseChild">
                  <img src="@/assets/img/icon/arrow-right.svg" alt="">
                </div>
              </div>
            </div>
          </div>
        </dropdown-component>
      </div>
    </div>

<!--    reservationBtn-->
    <div @click="openModal(true)" class="reserverenBtn field">
      <div class="field-content">
        <p v-if="edit">Bewerk</p>
        <p v-else>Reserveren</p>
      </div>
      <div class="field-action" >
        <img src="@/assets/img/icon/arrow-right.svg" alt="">
      </div>
    </div>

<!--    Modal-->
    <div style="position: absolute" v-if="modalOpen">
      <Modal ref="modal" title="Reservering" :fullscreen="true" style="position: absolute;" @close="closeModal()">
        <Lanes v-if="!finalReservationOverview"
               @selectedLanes="setSelectedLanes($event); resetCapacity()"
               :reservation="reservation" :date="date" :edit="false" />
        <ReservationComp v-if="finalReservationOverview"
            @confirmReservation="confirmReservation" @confirmReservationEmployee="confirmReservationEmployee"
            @finishEditReservation="finishEditReservation" @cancelReservation="cancelReservation"
            @updateSelectedUser="selectedUser = $event"
            :reservation-prop="reservation" :selected-user-prop="selectedUser" :date="date" :edit="edit" />
      </Modal>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import VueDatePicker from "@vuepic/vue-datepicker";
import dropdownComponent from "@/components/DropdownComponent.vue";
import Modal from "@/components/Modal.vue";
import Lanes from "@/components/Lanes.vue";
import Loader from "@/components/Loader.vue";
import ReservationOverview from "@/components/ReservationOverview.vue";
import { useStore, store } from '@/store/store'
import type { Reservation, ReservationLane, LaneReservationTime, User} from "@/store/types";
import {ReservationClient, UserClient, ApiException} from "../../../ApiClient";
import {roleMatch} from "@/store/auth-functions";
import {Roles} from "@/enums/Roles";
import ReservationComp from "@/components/Reservation.vue";

export default defineComponent({
  computed: {
    Roles() {
      return Roles
    }
  },
  components: {ReservationComp, VueDatePicker, dropdownComponent, Modal, Lanes, ReservationOverview, Loader},
  props: {
    edit: Boolean,
    editReservation: {} as any
  },
  data(){
    return {
      minDate: new Date(),
      date: new Date(new Date().setDate(new Date().getDate() + 1)),
      capacityDropdown: false,
      totalCapacity: 0,
      modalOpen: false,
      reservation: {
        reservationDate: new Date(),
        adults: 0,
        children: 0,
        isCanceled: false,
        lanes: [] as ReservationLane[],
      } as Reservation,
      finalReservationOverview: false,
      loading: false,
      selectedUser:{
        fullName: "",
        email: "",
      } as User,
    }
  },
  watch:{
    date(){
      this.reservation.lanes = [] as ReservationLane[]
    }
  },
  methods:{
    //handles open and close of dropdowns for lanes and capacity
    handleCapacityDropdownChange(state:boolean){
      this.capacityDropdown = state
    },

    closeModal(){
      this.modalOpen = false
    },

    increaseAdult(){
      //checks if total capacity valid
      if (!this.checkIfValidTotalCapacity()){
        this.$toast.info('Maximaal aantal personen bereikt')
        return
      }
      this.reservation.adults++
      this.totalCapacity++
    },

    decreaseAdult(){
      //number of selected lanes
      let lengthOfObject = Object.keys(this.reservation.lanes).length
      //checks if minimal limit of 0 is reached
      if (this.reservation.adults == 0){
        return
      }
      //removes all childs if there are not enough adults
      if (this.reservation.adults < (2*lengthOfObject)){
        this.reservation.children = 0
      }
      this.reservation.adults--
      this.totalCapacity--
    },

    increaseChild(){
      //number of selected lanes
      let lengthOfObject = Object.keys(this.reservation.lanes).length
      //checks if childs can be added
      if (!this.checkIfValidTotalCapacity() || this.reservation.adults == (7*lengthOfObject) && this.reservation.children == (2*lengthOfObject)){
        this.$toast.info('Maximaal aantal personen bereikt')
        return
      } else if(this.reservation.adults <= (1*lengthOfObject) ) {
        this.$toast.info(`Minimaal ${2*lengthOfObject} volwassenen`)
        return;
      }
      this.reservation.children++
      this.totalCapacity+= 0.5
    },

    decreaseChild(){
      //checks if minimal limit of 0 is reached
      if (this.reservation.children == 0) {
        return
      }
      this.reservation.children--
      this.totalCapacity-= 0.5
    },

    //checks if child capacity is valid
    checkIfChildIsValid(){
      this.totalCapacity = (this.reservation.children * 0.5) + (this.reservation.adults * 1) - 0.5
      if (!this.checkIfValidTotalCapacity() || this.reservation.children < 0 ){
        this.reservation.children = 0
        this.$toast.info("Dit zijn teveel kinderen voor het aantal geselecteerde banen")
      }
    },

    //checks if adult capacity is valid
    checkIfAdultIsValid(){
      this.totalCapacity = (this.reservation.children * 0.5) + (this.reservation.adults * 1) - 1
      if (!this.checkIfValidTotalCapacity() || this.reservation.adults < 0 ){
        this.reservation.adults = 0
        this.$toast.info("Dit zijn teveel volwassenen voor het aantal geselecteerde banen")
      }
    },

    //resets capacity
    resetCapacity(){
      this.reservation.adults = 0
      this.reservation.children = 0
      this.totalCapacity = 0
    },

    //checks if totalCapacity is valid
    checkIfValidTotalCapacity(){
      let lengthOfObject = Object.keys(this.reservation.lanes).length
      return !(this.totalCapacity >= 8*lengthOfObject || this.reservation.children + this.reservation.adults >= 10*lengthOfObject);
    },

    openModal(finalReservationOverview:boolean){
      //checks if user is logged in
      if (!this.checkIfLoggedIn()){
        return
      }
      //closes open dropdown
      this.capacityDropdown = false
      //sets if it needs to show reservation or the final reservation overview
      this.finalReservationOverview = finalReservationOverview
      if (finalReservationOverview) {
        if (!this.checkIfReservationIsValid()){
          return;
        } else if (!this.checkIfMinimumCapacityIsReached()){
          return;
        }
      }
      this.modalOpen = true;
      if (!finalReservationOverview){
        this.$nextTick(()=>{
          // this.checkAvailability();
          // this.selectChosenLanes();
        })
      }
    },

    //checks if minimal capacity for reservation is reached
    checkIfMinimumCapacityIsReached(){
      let lengthOfObject = Object.keys(this.reservation.lanes).length
      if((this.reservation.adults <= (1*lengthOfObject)) ) {
        this.$toast.info(`Minimaal ${2*lengthOfObject} volwassenen`)
        return false;
      }
      return true
    },

    //checks if user is logged in
    checkIfLoggedIn(){
      if (!store.getters.getIsLoggedIn){
        this.$toast.info("Je moet eerst inloggen voordat je een reservering kunt maken")
        return false
      }
      return true
    },

    //opens view reservationOverview in Reservation
    viewReservation(){
      if (!this.checkIfReservationIsValid()){
        return
      }
      document.querySelector(`.lanes`)?.classList.add('notActive')
      document.querySelector(`.confirmation`)?.classList.remove('notActive')
    },

    //confirms reservation as normal user and starts saving reservation
    confirmReservation(){
      this.saveReservation()
      this.resetReservation()
      let modal = this.$refs.modal as any
      modal.toggle()
    },

    //confirms reservation as employee and starts saving reservation
    confirmReservationEmployee(){
      if (!this.checkIfUserIsSet()){
        return
      }
      this.saveReservationEmployee()
      this.$toast.success("Reservering is gelukt")
      this.resetReservation()
      let modal = this.$refs.modal as any
      modal.toggle()
    },
    resetReservation(){
      this.resetCapacity()
      this.selectedUser = {
        fullName: "",
        email: "",
      } as User
      this.reservation = {
        reservationDate: new Date(),
        adults: 0,
        children: 0,
        lanes: [] as ReservationLane[],
      } as Reservation
    },

    //check if user is set, only used for employee
    checkIfUserIsSet(){
      if (this.selectedUser.fullName == "" || this.selectedUser.email == "") {
        this.$toast.info("Je moet eerst een gebruiker selecteren")
        return false
      }
      return true
    },

    //save reservation as normal user
    saveReservation(){
      this.loading = true
      this.reservation.reservationDate = this.date
      // @ts-ignore
      let reservationClient = new ReservationClient(null as any, this.http)
      reservationClient.createReservation(this.reservation).then((response) => {
        this.loading = false
        this.resetReservation()
        this.$toast.success("Reservering is gelukt")
      }).catch((msg : ApiException) => {
        this.loading = false
        this.$toast.error(msg.response)
      })
    },

    //save reservation as employee, difference from saveReservation is that it saves the reservation with a userId
    saveReservationEmployee(){
      this.loading = true
      this.reservation.reservationDate = this.date
      this.reservation.userId = this.selectedUser.id
      // @ts-ignore
      let reservationClient = new ReservationClient(null as any, this.http)
      // @ts-ignore
      reservationClient.createReservationWithUser(this.reservation).then((response) => {
        this.loading = false
        this.resetReservation()
        let modal = this.$refs.modal as any
        modal.toggle()
      })
    },

    //checks if reservation is valid by checking if lanes are selected
    checkIfReservationIsValid(){
      if (Object.keys(this.reservation.lanes).length == 0){
        this.$toast.info(`Je moet minimaal 1 baan kiezen`)
        return false
      }
      return true
    },

    //closes modal and resets reservation
    cancelReservation(){
      let modal = this.$refs.modal as any
      this.resetReservation()
      modal.toggle()
    },

    //finishes editing reservation and returns reservation to parent
    finishEditReservation() {
      let id = this.$route.params.id
      let modal = this.$refs.modal as any
      modal.toggle();
      setTimeout(() => {
        this.loading = true
        this.$emit('confirmReservation', this.reservation)
      }, 400)
    },

    //adds selected lans to reservation
    setSelectedLanes(lanes: ReservationLane[]){
      this.reservation.lanes = lanes
      //@ts-ignore
      this.$refs.modal.toggle()
    },

    //sets minimal date for datepicker
    setMinDate(){
      if (!roleMatch([Roles.ADMIN, Roles.EMPLOYEE])){
        this.minDate = new Date(new Date().setDate(new Date().getDate() + 1))
      }

    },

  },
  mounted(){
    if (this.$props.editReservation != undefined){
      this.reservation = this.$props.editReservation
      this.date = this.reservation.reservationDate
    }
    this.setMinDate();
  }
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
    height: 240px!important;
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

