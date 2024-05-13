<script lang="ts">
import { RouterView } from 'vue-router'
import {defineComponent} from "vue";
import Header from "./components/header.vue";
export default defineComponent({
  components:{ Header }
})
</script>

<template>
  <Header/>

  <RouterView />
</template>

<style scoped lang="scss">
@import './assets/css/colors.scss';

header {
  background-color: #530A08;
  width: 100%;
  height: 60px;
  color: $creme;
  font-family: Inter;
}

.darkText {
  color: $dark;
}

header main {
  width: 90%;
  margin-left: 5%;
  height: 100%;
  display: flex;
  justify-content: flex-end;
}

header main section {
  width: 40%;
  height: 100%;
}

.logOut {
  background-color: $red;
  border: none;
  border-radius: 5px;
  padding: 5px 10px;
  color: $creme;
  font-family: Inter;
}

.logOut:hover {
  cursor: pointer;
  background-image: linear-gradient(rgb(0 0 0/20%) 0 0);
}

.bars {
  height: 100%;
  display: flex;
}

.bar {
  height: 100%;
  width: 20px;
}

.verticalBar {
  width: 25px;
  height: 100px;
}

.verticalBar:nth-of-type(1) {
  background-color: $orange;
}

.verticalBar:nth-of-type(2) {
  background-color: $red;
}

.verticalBar:nth-of-type(3) {
  background-color: $creme;
}

.bar:nth-of-type(1) {
  background-color: $red;
}

.bar:nth-of-type(2) {
  background-color: $orange-2;
}

.info {
  height: calc(100% - 20px);
  width: 150px;
  background-color: $orange;
  padding: 10px;
  font-size: 14px;
}

.name {
  margin: 0;
}

.rightSide {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 10px;
}

.logoContainer {
  position: absolute;
  display: flex;
  left: 100px;
}

.logo {
  position: absolute;
  top: 25px;
  left: -48px;
  width: 225%;
  height: 200%;
  z-index: 100;
}

.links {
  display: flex;
  width: 40%;
  justify-content: flex-start;
  gap: 100px;
  align-items: center;
  margin-left: 10%;
  text-decoration: none;
}

.links > a:hover {
  color: lightgray;
  cursor: pointer;
}
</style>
