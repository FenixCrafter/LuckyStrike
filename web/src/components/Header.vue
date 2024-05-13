<template>
  <header>
    <main>
      <div :class="smallLogo ? 'smallLogo' : ''" @click="goHome" class="logoContainer">
        <div class="verticalBar"></div>
        <div class="verticalBar"></div>
        <div class="verticalBar"></div>
        <img alt="Luckystrike logo" class="logo" src="@/assets/img/luckystrike-logo.png" width="125" height="125" />
      </div>

      <section class="rightSide">
        <div class="nameContainer">
        <p class="name" @click="toggleDropdown" v-if="user.isLoggedIn">{{ $data.user.username }}
          <img src="@/assets/arrow-down.svg" alt="V">
        </p>
        </div>
        <div class="dropdown-controller" style="position:relative; top: -70%;right: -13%;" v-if="isOpen">
          <DropdownComponent class="dropdown-container" @close="toggleDropdown" :title="$data.user.username" :width="'250px'" :height="'auto'" :posX="'-200px'" :posY="'70px'">
            <div class="dropwdown-content">
              <div class="dropdown-item" @click="pushHome">Home</div>
              <div class="dropdown-item" @click="pushReservering">Reserveringen</div>
              <div v-if="roleMatch([Roles.ADMIN])" class="dropdown-item" @click="pushBezeting">Bezettings overzicht</div>
              <div v-if="roleMatch([Roles.ADMIN])" class="dropdown-item" @click="pushMedewerkers">Medewerker beheer</div>
              <div class="dropdown-item" @click="logOut">Log uit</div>
            </div>
          </DropdownComponent>
        </div>
        <button @click="logIn" v-if="!roleMatch([Roles.ADMIN, Roles.EMPLOYEE, Roles.CUSTOMER])" class="logOut">Log in</button>
        <div class="bars">
          <div class="bar"></div>
          <div class="bar"></div>
          <div class="info">
            <span class="darkText">Reserveer online of bel: </span><br class="Disable">0612345678
          </div>
        </div>
      </section>
    </main>
  </header>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
import type {User} from "@/store/types";
import { useStore, store } from '@/store/store'
import DropdownComponent from "@/components/DropdownComponent.vue";
import {useToast} from "vue-toast-notification";
import { Roles } from '@/enums/Roles';
import { roleMatch } from '@/store/auth-functions';
//@ts-ignore
import { JwtTokenDto, UserClient } from '!/ApiClient';

const $toast = useToast();
export default defineComponent({
  name: "Header",
  watch:{
    $route (to, from){
      this.refreshToken();
      if (to.path == "/registreer"){
        this.smallLogo = true;
      } else {
        this.smallLogo = false;
      }
    }
  },
  components:{
    DropdownComponent
  },
  data(){
    return{
      store: useStore(),
      user: {} as User,
      isOpen: false,
      smallLogo: false,
      Roles: Roles
    }
  },
  methods:{
    logOut(){
      this.isOpen = !this.isOpen
      new UserClient(null, this.http).revokeToken()
      .finally(() => store.commit('logOut'));
      this.$router.push('/')
    },
    logIn(){
      this.$router.push("/login")
    },
    goHome(){
      this.$router.push('/')
    },
    toggleDropdown(){
      this.isOpen = !this.isOpen
    },
    pushReservering(){
      this.$router.push('/reserveringen')
      this.isOpen = !this.isOpen
    },
    pushHome(){
      this.$router.push('/')
      this.isOpen = !this.isOpen
    },
    pushMedewerkers(){
      this.$router.push('/registerEmployee')
      this.isOpen = !this.isOpen
    },
    pushBezeting(){
      this.$router.push('/bezettingsDashboard')
      this.isOpen = !this.isOpen
    },
    refreshToken() {
      // @ts-ignore
      new UserClient(null, this.http).refreshToken()
      .then((jwtTokenDto?: JwtTokenDto) => {
        if (jwtTokenDto != null) {
          const token = jwtTokenDto.token;
          localStorage.setItem("Token", token)
          store.commit('logIn', token);
        } else {
          localStorage.removeItem("Token")
          store.commit('logOut');
        }
      })
      .catch((msg) => {
        localStorage.removeItem("Token")
        store.commit('logOut');
      })
    },
    roleMatch
  },
  mounted(){
    this.user = store.getters.getUser;
    if (localStorage.getItem("Token") == null)
      return;

    this.refreshToken();
  },
})
</script>

<style scoped lang="scss">
@import '@/assets/css/colors.scss';
.dropdown-container{
  z-index: 9999;
  color: black;
  .dropwdown-content{
    .dropdown-item{
      padding: 10px;
      border-bottom: 1px solid black;
      cursor: pointer;
    }
  }

  .dropdown-header{
    .close-container{
      .close-btn{
        width: 20px;
        height: 20px;
      }
    }
  }
}

.smallLogo {
  top: -25px;
  left: 30px !important;
  scale: 0.7;
}

header {
  background-color: #530A08;
  width: 100%;
  height: 60px;
  color: $creme;
  font-family: Inter, serif;
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
  font-family: Inter, serif;
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
  width: 170px;
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

  .name {
    cursor: pointer;
    text-decoration: underline;
  }
}

.nameContainer{
  //width: 110px;
  img{
    position: relative;
    top: 2px;
    width: 15px;
    //transform: translateX(-100%);
    height: 15px;
    filter: invert(1);
  }
}

.logoContainer {
  position: absolute;
  display: flex;
  left: 100px;
  z-index: 10000;
  cursor: pointer;
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

  @media (max-width: 1000px) {
   .logoContainer{
     display: none;
   }
    .Disable{
      display: none;
    }
    .dropdown-controller{
      right: -22% !important;
    }
  }



@media screen and (width < 1200px){
  .logoContainer{
    display: none;
  }
  .rightSide{
    width: 100%;
  }
}

@media screen and (width < 900px){
  .dropdown-controller{
    right: -30% !important;
  }
}

</style>