<template>
  <Loader v-if="showLoader"/>
  <div v-else class="container">
    <iframe :src="`https://localhost:7253/filestorage/pdf/rekening-${reservationId}.pdf`" title="Factuur"></iframe>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
import { ReservationClient } from '!/ApiClient';
import Loader from "@/components/loader.vue";

export default defineComponent({
  name: "Invoice",
  components: {Loader},
  data(){
    return{
      reservationId: 0,
      showLoader: true
    }
  },
  methods:{
    getReservationId(){
      this.reservationId = Number(this.$route.params.id)
    },
    async getPdf(){
      new ReservationClient(null, this.http).getPdf(this.reservationId);
    }
  },
  async mounted() {
    this.getReservationId()
    await this.getPdf()
    try {
      await new Promise(resolve => setTimeout(resolve, 1000));
    } catch (e) {
      
    }
    this.showLoader = false
  },
})
</script>

<style scoped lang="scss">
@import '/src/assets/css/colors.scss';
.container{
  width: 100%;
  height: calc(100vh - 55px);
  background-color: $creme;
  position: absolute;
  display: flex;
  justify-content: center;
  align-items: center;
  iframe{
    width: 80%;
    min-height: 100%;
  }
}

</style>