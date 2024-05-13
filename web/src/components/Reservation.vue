<template>
  <div v-if="!loading">
    <ReservationOverview
        v-if="!selectUser"
        @confirmReservation="$emit('confirmReservation')" @selectUser="selectUser = true" @confirmReservationEmployee="$emit('confirmReservationEmployee')" @finishEditReservation="$emit('finishEditReservation')" @cancelReservation="$emit('cancelReservation')"
        :reservation-prop="reservation" :selected-user-prop="selectedUser" :date="date" :edit="edit"/>
    <SelectUser v-if="selectUser" @updateSelectedUser="selectedUser = $event; $emit('updateSelectedUser', $event); selectUser = false"
        @showOverview="selectUser = false"
        @close="selectUser = false" :reservation="reservation" :date="date" :edit="edit"/>
  </div>

</template>

<script lang="ts">
import {defineComponent} from "vue";
import ReservationOverview from "@/components/ReservationOverview.vue";
import SelectUser from "@/components/SelectUser.vue";
import type {Reservation, User} from "@/store/types";

export default defineComponent({
  props: {
    reservationProp: {},
    selectedUserProp: {},
    edit: Boolean,
    date: Date,
  },
  components: {
    ReservationOverview,
    SelectUser
  },
  data() {
    return {
      loading: true,
      reservation: {} as Reservation,
      selectedUser: {} as User,
      selectUser: false,
    }
  },
  mounted() {
    //sets data
    this.reservation = this.$props.reservationProp as Reservation;
    this.selectedUser = this.$props.selectedUserProp as User;
    this.$nextTick(() => {
      this.loading = false
    })
  },
})
</script>