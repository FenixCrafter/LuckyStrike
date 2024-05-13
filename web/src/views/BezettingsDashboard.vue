<template>
  <Loader v-if="loading"></loader>
  <div class="container">
    <div class="datePicker">
      <vue-date-picker class="colorBlack" ref="datePicker" v-model="date" :enable-time-picker="false">{{ date.date }}
      </vue-date-picker>
    </div>
    <div><h3>{{fullDate}}</h3></div>
    <div v-if="show" class="modal-content-container lanes ">
      <button @click="dateDecrease" class="vorigeButton"><img src="src/assets/arrow-down.svg" alt="Vorige"></button>
      <div class="selectLaneContainer">
        <table class="modalTable">
          <tr>
            <th>
              <div style="width: 3rem; height: 3rem; display: flex; ">
                Start tijd
              </div>
            </th>
          </tr>
          <tr v-for="time in businessHours" class="modalTr">
            <td class="modalTd">{{ time }}</td>
          </tr>
        </table>
        <table class="modalTable" v-for="lane in totalLanes" :class="{'kidLane' : lane < 3}">
          <tr>
            <th>
              <div class="bowlingbalImg">{{ lane }}</div>
            </th>
          </tr>
          <tr class="modalTr" v-for="timeframe in businessHours">
            <td class="modalTd">
              <div :class="[`Box${lane}${timeframe}`, { 'magicBowlen': (timeframe == 22 || timeframe == 23) }]"
                   @click="goToReservation(`Box${lane}${timeframe}`)" class="avaiablelity"></div>
            </td>
          </tr>
        </table>
      </div>
      <button @click="dateIncrease" class="volgendeButton"><img src="src/assets/arrow-down.svg" alt="volgende"></button>

    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
import VueDatePicker from "@vuepic/vue-datepicker";
import type {Reservation, ReservationLane, LaneReservationTime} from "@/store/types";
import {ReservationClient} from "../../../ApiClient";
import loader from "@/components/loader.vue";
import {roleMatch} from "@/store/auth-functions";
import {Roles} from "@/enums/Roles";

export default defineComponent({
  name: "bezettingsDashboard",
  components: {VueDatePicker, loader},
  data() {
    return {
      show: true,
      businessHours: [] as number[],
      date: new Date(),
      occupiedLanes: [
        {
          id: 0,
          reservationId: 1,
          laneNumber: 1,
          timeframe: [
            {
              id: 1,
              reservationLaneId: 2,
              time: 15,
            }
          ] as LaneReservationTime[],
        }
      ],
      totalLanes: 8,
      loading: false,
      fullDate: "",
    }
  },
  watch: {
    date() {
      this.clearAvailability()
      this.checkIfWeekend()
      this.checkAvailability()
      this.getFullDateInString()
    },
  },
  mounted() {
    this.checkIfWeekend()
    //Checks if user is logged in and has admin role
    if (localStorage.getItem("Token") == null || !roleMatch([Roles.ADMIN])) {
      this.$router.push("/")
      this.$toast.error("Je hebt geen toegang tot deze pagina")
    }
    //waits for the dom to load
    if (this.show) {
      this.$nextTick(() => {
        this.checkAvailability()
      })
    }
    this.getFullDateInString()
  },
  methods: {
    //initial loop to check if lanes are available
    async checkAvailability() {
      let lanes = await this.getOccupiedLanes()
      Object.keys(lanes).forEach((lane) => {
        //loops through timeframe
        //@ts-ignore
        for (let timeframe of lanes[lane].timeframe) {
          //@ts-ignore
          let element = document.querySelector(`.Box${lanes[Number(lane)].laneNumber}${timeframe.time}`)!
          element.classList.add('bezet')
          element.id = lanes[lane].reservationId
        }
      })
    },
    //clears the availability of the lanes
    clearAvailability() {
      for (let lane = 1; lane <= this.totalLanes; lane++) {
        for (let timeframe in this.businessHours) {
          //@ts-ignore
          document.querySelector(`.Box${lane}${this.businessHours[timeframe]}`).classList.remove("bezet")
        }
      }
    },
    //gets occupied lanes from api
    async getOccupiedLanes() {
      this.loading = true
      let reservationClient = new ReservationClient(null as any, this.http)
      let response = await reservationClient.getOccupiedLanesWithReservationId(this.date)
      this.loading = false
      return response
    },
    //goes to reservation page
    goToReservation(reference: string) {
      if (document.querySelector(`.${reference}`).id != "") {
        this.$router.push(`/reservering/${document.querySelector(`.${reference}`).id}`)
      }
    },
    //decreases date by 1 day
    dateDecrease() {
      this.date = new Date(this.date.setDate(this.date.getDate() - 1))
    },
    //increases date by 1 day
    dateIncrease() {
      this.date = new Date(this.date.setDate(this.date.getDate() + 1))
    },
    //checks if day is weekend and changes business hours accordingly
    checkIfWeekend() {
      let date = new Date(this.date)
      if (date.getDay() == 0 || date.getDay() == 6) {
        this.businessHours = [14, 15, 16, 17, 18, 19, 20, 21, 22, 23]
        return
      }
      this.businessHours = [14, 15, 16, 17, 18, 19, 20, 21]
    },
    //gets full date in string
    getFullDateInString() {
      this.fullDate = `${this.getDay()} ${this.date.getDate()} ${this.getMonth()}`
    },
    //gets day in string
    getDay() {
      switch (this.date.getDay()) {
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
    //gets month in string
    getMonth() {
      switch (this.date.getMonth()) {
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
  },
})
</script>

<style scoped lang="scss">
@import "../assets/css/colors";

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

.modal-content-container {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: row;

  .information {
    width: 80%;
    margin: 0 auto;
    text-align: center;
  }

  .selectLaneContainer {
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


@media screen and (width < 920px) {
  .hero-booking-container {
    flex-direction: column;
    height: 370px;
    margin-bottom: 1% !important;
    width: 40%;
    justify-content: normal;
  }
  .field {
    height: 40px;
    width: 90% !important;
    margin-top: 5%;
  }

}

@media only screen and (max-width: 800px) {
  .hero-booking-container {
    width: 300px;
    height: 320px !important;
  }
  .field {
    height: 40px;
    width: 260px !important;
    margin-top: 5%;
  }
  .disable {
    display: block;
  }
  .information {
    margin-bottom: 0;
    transform: translateY(40%);
  }
  .maakReserveringBtn {
    margin: 0;
  }
}

@media only screen and (max-width: 630px) {
  .modalTable {
    margin-right: .1rem !important;
  }
}

@media only screen and (max-width: 630px) {
  .selectLaneContainer {
    transform: scale(0.8);
  }
}

@media only screen and (max-width: 410px) {
  .selectLaneContainer {
    transform: scale(0.7);
  }

  .confirmationButtons {
    flex-direction: column !important;
  }
}

.bezet {
  cursor: pointer !important;
  background: red !important;
}

.dropdown {
  z-index: 999 !important;
}

@import "src/assets/css/colors";
.container {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  height: 70vh;
}

.datePicker {
  //background: $orange;
  margin: auto 0;
  background: $gray-3;
  padding: 1rem;
  border-radius: 0 0 1rem 1rem;
}

.volgendeButton, .vorigeButton {
  border: none;
  cursor: pointer;
  border-radius: 5rem;
  width: 3rem;
  height: 3rem;
  background: $orange;
  display: flex;
  align-items: center;
  justify-content: center;

  img {
    width: 50%;
  }
}

.volgendeButton {
  transform: rotate(-90deg);
}

.vorigeButton {
  transform: rotate(90deg);

}
</style>
