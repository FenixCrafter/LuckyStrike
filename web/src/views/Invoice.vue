<template>
  <Loader v-if="showLoader"/>
  <div v-else class="container">
    <iframe :src="`https://localhost:7253/filestorage/pdf/rekening-${reservationId}.pdf`" title="Factuur"></iframe>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
import { ReservationClient, ApiException } from '!/ApiClient';
import Loader from "@/components/Loader.vue";

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
    //Gets reservation id from url
    getReservationId(){
      this.reservationId = Number(this.$route.params.id)
    },
    //Makes api call to get pdf
    async getPdf(){
      new ReservationClient(null, this.http).getPdf(this.reservationId)
          .then(() => {
            console.log("Factuur opgehaald")
          }).catch((msg: any) => {
      })
    }
  },
  async mounted() {
    this.getReservationId()
    await this.getPdf()
    try {
      //Wait for pdf to be generated
      await new Promise(resolve => setTimeout(resolve, 1000));
    } catch (e) {
      this.$toast.error("Factuur niet gevonden")
    }
    this.showLoader = false
  },
})
</script>

<style scoped lang="scss">
@import '/src/assets/css/colors.scss';
.container{
  width: 100%;
  height: calc(100vh - 60px);
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