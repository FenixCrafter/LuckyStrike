<template>
  <loader v-if="loading" />
  <header>
    <main>
      <div class="logoContainer">
        <div class="verticalBar"></div>
        <div class="verticalBar"></div>
        <div class="verticalBar"></div>
        <img alt="Luckystrike logo" class="logo" src="@/assets/luckystrike-logo.png" width="125" height="125" />
      </div>
      <section class="rightSide">
        <router-link v-if="store.getters.getIsLoggedIn" class="header-item" to="/overview">Producten bestellen</router-link>
        <router-link v-if="roleMatch([Roles.ADMIN])" class="header-item" to="/producten-beheer">Producten beheeren</router-link>
        <router-link v-if="roleMatch([Roles.ADMIN, Roles.EMPLOYEE])" class="header-item" to="/management">Orders beheeren</router-link>
        <router-link v-if="roleMatch([Roles.ADMIN, Roles.EMPLOYEE])" class="header-item" to="/rekeningInput">Rekening</router-link>
        <router-link @click="handleLogout" v-if="store.getters.getIsLoggedIn" class="header-item" to="/">Log uit</router-link>
        <div class="bars">
          <div class="bar"></div>
          <div class="bar"></div>
          <div class="info">
            <span class="darkText">Problemen?</span><br>Meld het aan de balie
          </div>
        </div>
      </section>
    </main>
  </header>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import { useStore, store } from '@/store/store';
//@ts-ignore
import { JwtTokenDto, UserClient } from '!/ApiClient';
import { Roles } from '@/enums/Roles';
import { roleMatch } from '@/store/auth-functions';
import loader from "@/components/loader.vue";

export default defineComponent({
  components: {
    loader
  },
  data(){
    return{
      loading: false,
      store: useStore(),
      Roles: Roles
    }
  },
  watch:{
    $route (){
      this.refreshToken();
    }
  },
  methods:{
    handleLogout(){
      this.loading = true;
      //@ts-ignore
      new UserClient(null, this.http).revokeToken()
      .finally(() => {
        this.loading = false;
        store.commit('logOut');
      });
    },
    refreshToken() {
      //@ts-ignore
      new UserClient(null, this.http).refreshToken()
        .then((jwtTokenDto?: JwtTokenDto) => {
          if (jwtTokenDto != null) {
            const token = jwtTokenDto.token;
            localStorage.setItem("Token", token)
            store.commit('logIn', token);
          } else {
            store.commit('logOut');
            this.$router.push("/");
          }
        })
        .catch(() => {
          store.commit('logOut');
          this.$router.push("/");
        })
    },
    roleMatch
  },
  mounted() {
    if (localStorage.getItem("Token") == null)
      return;

    this.refreshToken();
  }
})
</script>

<style scoped lang="scss">
@import '@/assets/css/colors.scss';

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
  width: 90%;
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
  width: 10%;
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
.header-item{
  //width: 300px;
  color: white;
  text-decoration: none;
  margin-left: .5rem;

  &:hover{
    text-decoration: underline;
  }
}

</style>
