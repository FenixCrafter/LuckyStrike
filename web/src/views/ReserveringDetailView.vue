<template>
  <Loader v-if="loading"></loader>
  <reserveringDetail v-if="!loading" :reservation-prop="reservation" :user-prop="user" />
</template>

<script lang="ts">
import {defineComponent} from "vue";
import reserveringDetail from "@/components/ReserveringDetail.vue"
import store from "@/store/store";
import {useToast} from "vue-toast-notification";
import {ReservationClient, UserClient} from "../../../ApiClient";
import loader from "@/components/Loader.vue";
import type {Reservation, User} from "@/store/types";
const $toast = useToast();
export default defineComponent({
  components: {reserveringDetail, loader},
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

<style lang="scss">
@import "@/assets/css/colors.scss";

</style>