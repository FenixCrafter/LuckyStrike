<template>
  <div class="titleCardContainer">
    <h2 class="title">Wij staan klaar voor jouw avondje uit!</h2>
    <h3 class="subTitle">Want als onze gasten genieten, dan genieten wij ook!</h3>
  </div>
</template>
<script lang="ts">
import {defineComponent} from "vue";

export default defineComponent({

})
</script>

<style scoped lang="scss">
//@import url("@/assets/css/colors.scss");
@import "@/assets/css/colors.scss";
.titleCardContainer{
  padding-top: 10px;
  padding-bottom: 10px;
  width: 100%;
  height: 100%;
  background-color: $orange;
  min-height: 150px;
  text-align: center;
}
h2{
  margin: 0 auto;
  width: 80%;
  font-size: 40px;
  font-weight: bold;
  background-color: $orange;
  padding-top: 10px;
  color: white;
  padding-left: 1rem;
  padding-right: 1rem;
}
h3{
  margin: 1.2rem auto 0;
  width: 80%;
  text-align: center;
  font-size: 20px;
  font-weight: bold;
  background-color: $orange;
  padding-bottom: 10px;
}
@media only screen and (max-width: 650px) {
  h2{
    font-size: 1rem !important;
    //padding: 0 2rem 0 2rem;
  }
}

@media only screen and (max-width: 800px) {
  h2 {
    font-size: 2rem !important;
    //padding: 0 2rem 0 2rem;
  }
}
@media only screen and (max-width: 1050px) {
  h2{
    font-size: 3rem;
    //padding: 0 2rem 0 2rem;
  }
}
</style>