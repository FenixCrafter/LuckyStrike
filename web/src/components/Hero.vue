<template>
<div class="hero-container">
  <h1 class="hero-title">Bowlingbaan Lucky Strike</h1>
  <Booking class="heroBooking"/>
</div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import Booking from "@/components/Booking.vue";
export default defineComponent({
  components:{
    Booking
  }
})
</script>

<style scoped lang="scss">
@import "../assets/css/colors";
.hero-container{
  width: 100vw;
  height: 40vw;
  background-image: url("../assets/img/hero.png");
  background-repeat: no-repeat;
  background-size: cover;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  padding: 2rem;
}

.hero-title{

  margin-top: 10%;
  font-size: 5rem;
  color: $creme;
  text-align: center;
}
.heroBooking{
  margin-bottom: 3%;
  top: 70%;
}

.titleCardContainer{
  padding: 25px;
  width: 100%;
  height: 100%;
  background-color: $orange;
  min-height: 150px;
  text-align: center;
  h1{
    font-size: 60px;
    font-weight: bold;
    background-color: $orange;
    padding-top: 10px;
    color: white;
  }
  h2{
    font-size: 30px;
    font-weight: bold;
    background-color: $orange;
    padding-bottom: 10px;
  }
}

@media screen and (width < 1200px){
  .hero-container{
    padding: 0!important;;
  }
}
@media only screen and (max-width: 1050px){
  .hero-container{
    height: 50vw;
  }
  .hero-title{
    font-size: 3rem;
  }
}
@media only screen and (max-width: 920px) {
  .hero-container {
    height: 52vw !important;
  }
}
@media only screen and (max-width: 800px) {
  .hero-container {
    height: 52vh !important;
    justify-content: center;
  }
  .titleCardContainer{
    padding: 25px 0;
  }
  .hero-title{
    font-size: 2rem !important;
  }
}

@media only screen and (max-width: 670px) {
.hero-title{
  font-size: 3rem;
}
}

@media only screen and (max-width: 400px){
  .hero-title{
    font-size: 2rem;
  }
  .titleCardContainer{
    h1{
      font-size: 25px;
    }
    h2{
      font-size: 15px;
    }
  }
}
</style>