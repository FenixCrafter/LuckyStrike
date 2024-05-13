<template>
  <Loader v-if="loading"></loader>
  <h1 v-if="roleMatch([Roles.CUSTOMER])" class="title">Jouw reserveringen</h1>
  <div class="dateContainer" v-if="!roleMatch([Roles.CUSTOMER])">
    <h1 class="title">Alle reserveringen</h1>
    <div class="datePickerContainer">
      <button @click="dateDecrease" class="vorigeButton"><img src="src/assets/arrow-down.svg" alt="Vorige"></button>
      <vue-date-picker class="datePicker" v-model="date" :enable-time-picker="false"></vue-date-picker>
      <button @click="dateIncrease" class="volgendeButton"><img src="src/assets/arrow-down.svg" alt="volgende"></button>
    </div>
  </div>
  <div class="fullDateString" v-if="!roleMatch([Roles.CUSTOMER])"><h3>{{fullDate}}</h3></div>
  <div class="container">
    <div class="reservations">
      <div v-if="!loading">
        <div v-if="reservations.length!= 0" class="reservation" v-for="reservation in reservations" :key="reservation.id"
             @click="goToInfoPage(Number(reservation.id))">
          <span class="reservation__id">#{{ reservation.id }}</span>
          <span v-if="(roleMatch([Roles.ADMIN]) || roleMatch([Roles.EMPLOYEE]))"
                class="reservation__name">{{reservation.userName}}</span>
          <span v-for="baan in reservation.lanes" class="reservation__baan">{{ baan.laneNumber }}</span>
          <br class="disable">
          <span class="reservation__date"> <img alt="Date" src="src/assets/clock.svg"> {{ reservation.reservationDate.toLocaleDateString() }}</span>
          <span class="reservation__amount"><img alt="Person"
                                                 src="src/assets/user.svg"> {{ (reservation.adults + reservation.children) }}</span>
          <span class="reservation__amount">€{{calculateReservationCosts(reservation.lanes)}}</span>
        </div>
        <div class="noReservation" v-else>
          <h3>Geen reserveringen gevonden</h3>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
import type {User, Reservation, ReservationLane} from "@/store/types";
import {useStore, store} from '@/store/store'
import {useToast} from "vue-toast-notification";
import {Roles} from '@/enums/Roles';
import {roleMatch} from '@/store/auth-functions';
import {ReservationClient} from "../../../ApiClient";
import loader from "@/components/Loader.vue";
import VueDatePicker from "@vuepic/vue-datepicker";
const $toast = useToast();

export default defineComponent({
  name: "ReservationDashboard",
  components: {loader, VueDatePicker},
  data() {
    return {
      store: useStore(),
      user: {} as User,
      reservations: [] as Reservation[],
      Roles: Roles,
      loading: true,
      date: new Date(),
      fullDate: "",
    }
  },
  watch: {
    date() {
      this.getReservations()
      this.getFullDateInString()
    }
  },
  methods: {
    goToInfoPage(reservationId: number) {
      this.$router.push(`/reservering/${reservationId}`)
    },
    async getReservations() {
      this.loading = true
      //@ts-ignore
      let reservationClient = new ReservationClient(null as any, this.http)
      if (roleMatch([Roles.CUSTOMER])) {

        reservationClient.getUserReservations().then((reservations: Reservation[]) => {
          this.reservations = reservations
          this.loading = false
        }).catch((msg: any) => {
          $toast.error("Reserveringen ophalen mislukt, probeer het later opnieuw")
          this.loading = false
        })
      } else {
        reservationClient.getReservationByDate(this.date).then((reservations: Reservation[]) => {
          this.reservations = reservations
          this.loading = false
        }).catch((msg: any) => {
          $toast.error("Reserveringen ophalen mislukt, probeer het later opnieuw")
          this.loading = false
        })
      }
      this.reservations.reverse()
    },
    getFullDateInString(){
      this.fullDate = `${this.getDay()} ${this.date.getDate()} ${this.getMonth()}`
    },
    getDay(){
      switch (this.date.getDay()){
        case 0:
          return "Zondag"
        case 1:
          return "Maandag"
        case 2:
          return "Dinsdag"
        case 3:
          return "Woensdag"
        case 4:
          return "Donderdag"
        case 5:
          return "Vrijdag"
        case 6:
          return "Zaterdag"
      }
    },
    getMonth(){
      switch (this.date.getMonth()){
        case 0:
          return "Januari"
        case 1:
          return "Februari"
        case 2:
          return "Maart"
        case 3:
          return "April"
        case 4:
          return "Mei"
        case 5:
          return "Juni"
        case 6:
          return "Juli"
        case 7:
          return "Augustus"
        case 8:
          return "September"
        case 9:
          return "Oktober"
        case 10:
          return "November"
        case 11:
          return "December"
      }
    },
    dateDecrease() {
      this.date = new Date(this.date.setDate(this.date.getDate() - 1))
    },
    dateIncrease() {
      this.date = new Date(this.date.setDate(this.date.getDate() + 1))
    },
    calculateReservationCosts(lanes: ReservationLane[]){
      if (lanes.length == 0){
        return 0
      }
      let day = this.date.getDay()
      let totalCost = 0
      //checks if day is monday, tuesday, wednesday or thursday
      if (day >= 1 && day <= 4){
        //loops through selected Lanes
        lanes.forEach((lane:ReservationLane) =>{
          //because price is the same no matter the hour of these days,
          //the length of the timeframe can be multiplied by 24 euros
          totalCost += (lane.timeframe.length * 24)
        })
      } else {
        //loops through selected Lanes
        lanes.forEach((lane:ReservationLane) =>{
          //loops through timeframe and adds costs based on time of timeframe
          for (let timeframe of lane.timeframe){
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
    roleMatch
  },
  async mounted() {
    this.user = await store.getters.getUser
    if (!this.$data.user.isLoggedIn) {
      $toast.error('U moet ingelogd zijn om deze pagina te bekijken')
      this.$router.push('/')
    }
    await this.getReservations()

    this.getFullDateInString()
  },

})
</script>

<style scoped lang="scss">
@import '@/assets/css/colors.scss';

.disable {
  display: none;
}

.title {
  margin-left: 5%;
  margin-top: 5%;
  font-family: Inter, serif;
  font-size: 2rem;
  position: relative;
  left: 120px;
}

.container {
  width: 100%;
  height: 100%;
  background-color: $creme;
  position: relative;
  left: 3%;
}

.reservations {
  color: white;
  width: 100%;
  height: 100%;
  display: flex;
  flex-wrap: wrap;
  //justify-content: wrap;
  align-items: center;
  div{
    width: 95%;
  }
}

.reservation {
  width: 90%;
  min-height: 55%;
  border-radius: 25px;
  margin: 1rem !important;
  background: $gray-3;
  color: $dark;
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  align-items: center;
  cursor: pointer;

  span {
    margin: 10px;
    font-family: Inter, serif;
    text-align: center;
    font-size: 1.5rem;

    img {
      //filter: invert(1);

      position: relative;
      top: 5px;
      width: 25px;
      height: 25px;
    }
  }

  .reservation__id {
    width: 50px;
    font-family: Inter, serif;
    font-size: 1.5rem;
    margin: 20px;
    display: inline-block;
  }

  .reservation__baan {
    display: flex;
    justify-content: center;
    align-items: center;
    text-align: center;
    width: 40px;
    height: 40px;
    font-weight: bold;
    color: white;
    font-size: 1.5rem;
    border-radius: 50%;
    background-image: url("@/assets/img/icon/image 11.png");
    background-size: cover;
    background-repeat: no-repeat;
  }
}

@media only screen and (max-width: 1600px) {
  .title {
    left: 180px;
  }
  .container {
    left: 20px;
  }
  .reservations {
    margin-top: 80px;
  }
  .reservation {
    width: 90%;
    height: 40%;
  }
}

@media only screen and (max-width: 1200px) {
  .title {
    left: 0;
  }
  .reservations {
    margin-top: 0;
  }
}

@media only screen and (max-width: 1000px) {
  .title {
    left: 10px;
  }

}

.dateContainer {
  display: flex;
  align-items: center;
  //justify-content: center;
  width: 100%;
  margin: 4rem 0;

  .datePickerContainer {
    display: flex;
    //justify-content: center;
    //align-items: center;
    flex-direction: row;
    width: 50%;
  }

  .title {
    margin: 0;
    padding: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 50%;
    margin-right: 1.5rem;
  }
}

.datePicker {
  //background: $orange;
  margin: auto 0;
  //background: $gray-3;
  width: 50%;
  margin: 0 1rem 0 1rem;
}

.volgendeButton, .vorigeButton {
  border: none;
  cursor: pointer;
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 50%;
  background: $orange;
  display: flex;
  align-items: center;
  justify-content: center;

  img {
    width: 80%;
  }
}

.volgendeButton {
  transform: rotate(-90deg);
}

.vorigeButton {
  transform: rotate(90deg);

}

@media only screen and (max-width: 800px) {
  .dateContainer {
    width: 90%;
    padding: 0 2rem 0 2rem !important;
    flex-direction: column;
  }
  .datePickerContainer {
    width: 90% !important;
    justify-content: center;

    .datePicker {
      width: 80%;
    }
  }
  .title {
    margin-bottom: .5rem !important;
    font-size: 1.5rem !important;
    width: 100% !important;
  }

}
.noReservation{
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  h2{
    font-size: 2rem;
    font-family: Inter, serif;
  }
}
.fullDateString{
  width: 100%;
  h3{
    text-align: center;
    width: 100%;
  }
}
</style>
