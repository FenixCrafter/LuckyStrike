<template>
  <div v-if="!loading" class="infoContainer">
    <div><h2>Reservatie:</h2></div>
    <div><p>Reservatie nummer: {{reservation.id}}</p></div>
    <div><p>Datum: {{reservation.reservationDate.toLocaleDateString()}}</p></div>
    <div><p>Naam: {{user.fullName}}</p></div>
    <div><p>Adres: {{user.street}}</p></div>
    <div><p>Plaats: {{user.city}}, {{user.zipcode}}</p></div>
    <div><p>Email: {{user.email}}</p></div>
    <div><p>telefoon nummer: {{user.phoneNumber}}</p></div>
    <div><p>Aantal volwassenen: {{reservation.adults}}</p></div>
    <div><p>Aantal kinderen: {{reservation.children}}</p></div>
    <div>
      <p>Geselecteerde Banen: </p>
      <div class="selectedLanesAndTimes" v-for="lane in reservation.lanes " :class="{'kidLane': lane.laneNumber <= 2}">
        <div class="laneNumber">{{lane.laneNumber}}</div>
        <p>{{getLaneTimeframe(lane.timeframe)}}</p>
        <p style="margin-left: 1rem" v-if="lane.laneNumber <= 2">Kinderbaan</p>
      </div>
    </div>
    <div><p>Kosten: €{{calculateReservationCosts()}}</p></div>
    <div v-if="!noButtons" class="reservationBtns">
      <button class="bekijkFactuurBtn" @click="goToInvoice">Bekijk Factuur</button>
      <button v-if="checkIfReservationDateIsInThePast()" class="bekijkFactuurBtn" :class="{'disabled' : checkIfReservationDateIsToday()}" @click="handleGoToEditPage">Bewerk reservatie</button>
      <button v-if="checkIfReservationDateIsInThePast()" class="annuleerBtn" :class="{'disabled' : checkIfReservationDateIsToday()}" @click="handleAnnuleerModal()">Annuleer reservatie</button>
    </div>
  </div>
  <div v-if="modal" class="confirmPopUp">
    <modal ref="cancelModal" @close="modal = false" title="Annuleer reservering"  width="500">
      <div class="modalConfirmContainer">
        <h2>Weet u zeker dat u uw reservering wilt annuleren?</h2>
        <div class="confirmationButtonContainer">
          <div class="confirmBtn btnYes" @click="cancelReservation">Ja</div>
          <div class="confirmBtn btnNo" @click="closeModal">Nee</div>
        </div>
      </div>
    </modal>
  </div>

</template>

<script lang="ts">
import {defineComponent} from "vue";
import modal from "@/components/Modal.vue";
import type {LaneReservationTime, Reservation, User, ReservationLane} from "@/store/types";
import {ReservationClient, UserClient} from "../../../ApiClient";
import {roleMatch} from "@/store/auth-functions";
import {Roles} from "@/enums/Roles";

export default defineComponent({
  components:{modal},
  props: {
    reservationProp:  {},
    userProp: {},
    noButtons: Boolean
  },
  data(){
    return{
      modal: false,
      reservation: {} as Reservation,
      user: {} as User,
      loading: true,
    }
  },
  methods:{
    //calculates timeframe of lane and returns it as a string
    getLaneTimeframe(timeframe:LaneReservationTime[]) {
      if (timeframe.length == 1){
        return `${timeframe[0].time}:00 - ${timeframe[0].time +1}:00`
      } else {
        return `${Math.min(...timeframe.map(time => time.time))}:00 - ${Math.max(...timeframe.map(time => time.time))+1}:00`
      }
    },
    //goes to invoice page
    goToInvoice() {
      this.$router.push(`/reservering/${this.reservation.id}/factuur`)
    },
    goToEdit() {
      this.$router.push(`/reservering/${this.reservation.id}/bewerk`)
    },
    //cancels reservation
    cancelReservation() {
      //@ts-ignore
      new ReservationClient(null, this.http).cancelReservation(this.reservation.id)
          .then(() => {
            this.$toast.success("Reservering geannuleerd")
            this.$router.push('/reserveringen')
          }).catch((msg: any) => {
        this.$toast.error("Reservering annuleren mislukt, probeer het later opnieuw")

      })
    },
    //closes modal
    closeModal() {
      this.$refs.confirmModal.toggle()
    },
    //calculates costs of reservation
    calculateReservationCosts(){
      let day = new Date(this.reservation.reservationDate).getDay()
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
    //checks if reservation date is today
    checkIfReservationDateIsToday(){
      if (this.reservation.reservationDate.getDate() == new Date().getDate() && roleMatch([Roles.CUSTOMER])){
        return true
      }
      return false
    },
    //checks if reservation date is in the past
    checkIfReservationDateIsInThePast(){
      //@ts-ignore
      if (this.reservation.reservationDate < new Date().setDate(new Date().getDate() - 1)){
        return false
      }
      return true
    },
    // Checks if reservation date is today, if so, shows error message else opens modal
    handleAnnuleerModal(){
      if (this.checkIfReservationDateIsToday()){
        this.$toast.error("Reservering kan niet geannuleerd worden via de website op de dag van de reservering, neem contact op met de bowlingbaan")
      } else {
        this.modal = true
      }
    },
    // Checks if reservation date is today, if so, shows error message else goes to edit page
    handleGoToEditPage(){
      if (this.checkIfReservationDateIsToday()){
        this.$toast.error("Reservering kan niet bewerkt worden via de website op de dag van de reservering, neem contact op met de bowlingbaan")
      } else {
        this.$router.push(`/reservering/${this.reservation.id}/bewerk`)
      }
    }
  },
  async mounted(){
    this.reservation = await this.reservationProp as Reservation
    this.user = await this.userProp as User
    //waits for next tick to set loading to false
    this.$nextTick(() => {
      this.loading = false
    })
  }
})

</script>

<style lang="scss">
@import "@/assets/css/colors.scss";
.infoContainer{
  width: 20%;
  margin: 0 auto 10px;
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
  .reservationBtns{
    width: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;

    gap: 10px;
    > * {
      cursor: pointer;
      width: 100% !important;
    }
    .bekijkFactuurBtn {
      display: flex;
      justify-content: center;
      background: $dark;
      width: 40%;
      color: $creme;
      border: none;
      padding: 1rem;
      border-radius: 10px;
      //margin-right: .4rem;
    }

    .annuleerBtn{
      background: transparent;
      display: flex;
      justify-content: center;
      align-items: center;
      width: calc(50% - 4px);
      padding: .9rem;
      border-radius: 10px;
      border: 2px solid $red;
      color: $red;
      cursor: pointer;
    }
    .disabled{
      opacity: 0.3;
      cursor: not-allowed;
    }
  }
  .confirmPopUp{
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
  }
.modalConfirmContainer{
  padding: 0 2rem 2rem 2rem;
  width: 87%;
  height: 80%;

  .confirmationButtonContainer{
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: space-around;

    .confirmBtn{
      width: 30%;
      padding: 2rem;
      border-radius: 10px;
      display: flex;
      justify-content: center;
      align-items: center;
      font-weight: bold;
      cursor: pointer;
    }
    .btnYes{
      background: $green;
      color: $dark;
    }
    .btnNo{
      background: $red;
      color: $creme;
    }
  }

}
@media only screen and (max-width: 1230px){
  .infoContainer{
    width: 60%;
    margin: 0 auto 2rem auto;
  }
}
@media only screen and (max-width: 1000px){
  .infoContainer{
    width: 80%;
  }
}
</style>