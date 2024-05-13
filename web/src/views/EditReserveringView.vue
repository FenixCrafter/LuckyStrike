<template>
  <Loader v-if="loading"></loader>
  <main v-else>
    <reserveringDetail noButtons :reservation-prop="reservation" :user-prop="user" />
    <HeroBooking @confirmReservation="confirmReservation($event)" edit :editReservation="reservation" />
  </main>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import reserveringDetail from "@/components/ReserveringDetail.vue"
import store from "@/store/store";
import {useToast} from "vue-toast-notification";
import {ReservationClient, UserClient} from "../../../ApiClient";
import {EditReservationModel, ReservationLane, LaneReservationTime} from "../../../ApiClient";
import loader from "@/components/Loader.vue";
import type {Reservation, User} from "@/store/types";
import HeroBooking from "@/components/Booking.vue";
const $toast = useToast();
export default defineComponent({
  components: {HeroBooking, reserveringDetail, loader},
  data(){
    return{
      reservation: {
      } as Reservation,
      user: {} as User,
      reservationId: Number(this.$route.params.id),
      loading: false
    }
  },
  methods:{
    async getReservation(){
      this.loading = true
      // @ts-ignore
      let reservationClient = new ReservationClient(null as any, this.http)
      // @ts-ignore
      this.reservation = await reservationClient.getReservation(this.reservationId)

      await this.getUser(this.reservation.userId)
      this.loading = false
    },

    async getUser(userId:string){
      // @ts-ignore
      let userClient = new UserClient(null as any, this.http)
      // @ts-ignore
      this.user = await userClient.getUser(userId)
    },
    confirmReservation(reservation: Reservation){
      this.loading = true

      let lanes: ReservationLane[] = [];

      for (let lane of reservation.lanes) {
        let timeframes: LaneReservationTime[] = [];
        for (let timeframe of lane.timeframe) {
          timeframes.push(new LaneReservationTime({
            id: timeframe.id!,
            reservationLaneId: timeframe.reservationLaneId!,
            time: timeframe.time
          }) as LaneReservationTime);
        }
        lanes.push(new ReservationLane({
          id: lane.id!,
          laneNumber: lane.laneNumber,
          reservationId: lane.reservationId!,
          timeframe: timeframes
        }) as ReservationLane);
      }
      //@ts-ignore
      let reservationModel = new EditReservationModel({
        reservationDate: reservation.reservationDate,
        adults: reservation.adults,
        children: reservation.children,
        lanes: lanes
      })
      //@ts-ignore
      new ReservationClient(null, this.http).updateReservation(this.reservationId, reservationModel)
          .then(() => {
            this.loading = false
            this.$toast.success("Reservering is aangepast")
            this.$router.push(`/reservering/${this.reservationId}`)
            this.getReservation()
          }).catch((msg: any) => {
        this.$toast.error("Reservering is niet aangepast")
      })
    }
  },
  async mounted(){
    let user = store.state.user
    if (!user.isLoggedIn){
      $toast.error('U moet ingelogd zijn om deze pagina te bekijken')
      this.$router.push('/')
    }
    await this.getReservation()
  },
})

</script>

<style lang="scss" scoped>
@import "@/assets/css/colors.scss";

main {
  display: flex;
  flex-direction: column;
  //justify-content: center;
  align-items: center;
  width: 100%;
  height: 90vh;
}
</style>