<script setup lang="ts">
import { RouterView } from 'vue-router'
import Footer from '@/components/Footer.vue'
import Header from '@/components/Header.vue'
import 'vue-toast-notification/dist/theme-sugar.css';

</script>

<template>
  <Header />
  <div class="container">
  <RouterView />
  </div>
  <Footer class="footer" />
</template>


<script lang="ts">
import {VueElement, defineComponent, provide} from "vue";

// import hero from "@/components/Hero.vue";


export default defineComponent({
  components:{
    // hero
  }
})
</script>


<style scoped lang="scss">
@import './assets/css/colors.scss';
.container{
  width: 100%;
  height: 100%;
  background-color: $creme;
  min-height: calc(100vh - 410px);
  overflow-x: hidden;
  z-index: 999;
}
.footer{
  z-index: 1;
}
@media only screen and (max-width: 920px){
  .container{
    min-height: calc(100vh - 450px);
  }
}
</style>