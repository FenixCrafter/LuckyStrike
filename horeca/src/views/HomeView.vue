<template>
  <loader v-if="loading" />
  <main v-if="adminLogin">
      <h1>Login</h1>
      <div class="inputField">
        <input type="text" v-model="user.email" placeholder="email">
      </div>
      <div class="inputField">
        <input type="password" v-model="user.password" placeholder="wachtwoord">
      </div>
      <button @click="onAdminLogin">Log In</button>
      <p class="invalidMessage" v-if="invalidAccount">De combinatie van e-mailadres en wachtwoord is niet geldig.</p>
      <p class="toggleLoginViews" @click="adminLogin = !adminLogin">Log in met code</p>
    </main>
  <main v-else>
      <h1>Code</h1>
      <div class="inputField">
        <input type="text" maxlength="4" v-model="code" placeholder="code">
      </div>
      <button @click="onCodeLogin">{{ rekeningInput ? "Zie rekening" : "Log In" }}</button>
      <p class="invalidMessage" v-if="invalidCode">De code is niet geldig.</p>
      <p class="toggleLoginViews" v-if="!rekeningInput" @click="adminLogin = !adminLogin">Ben je een medewerker of admin?</p>
  </main>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import {store} from "@/store/store";
import { UserClient, JwtTokenDto, LoginModel, ReservationIdDto } from '!/ApiClient';
import loader from "@/components/loader.vue";
import { ApiException } from '!/ApiException';
import { useToast } from 'vue-toast-notification';

export default defineComponent({
  components: {
    loader
  },
  data() {
    return {
      loading: false,
      user: {
        email: "",
        password: ""
      },
      code: "",
      adminLogin: false,
      invalidAccount: false,
      invalidCode: false,
      rekeningInput: false
    }
  },
  watch:{
    $route (){
      this.invalidCode = false;
      this.invalidAccount = false;
      this.code = "";
      this.user = {
        email: "",
        password: ""
      };
      if (this.$router.currentRoute.value.fullPath == "/rekeningInput"){
        this.rekeningInput = true;
      } else {
        this.rekeningInput = false;
      }
    }
  },
  methods: {
    $toast: useToast(),
    onAdminLogin() {
      this.invalidAccount = false;
      this.loading = true;
      new UserClient(null, this.http).login(new LoginModel({
        email: this.user.email,
        password: this.user.password
      })).then((tokenDto: JwtTokenDto) => {
        let token = tokenDto.token;
        localStorage.setItem("Token", token)
        store.commit('logIn', token);
        this.$router.push("/overview");
      }).catch((err: ApiException) => {
        this.invalidAccount = true;
        this.$toast.error(err.response);
      })
          .finally(() => {
            this.loading = false;
          })
    },
    onCodeLogin() {
      this.invalidCode = false;
      if (this.rekeningInput){
        new UserClient(null, this.http).getPdfWithReservationCode(this.code)
            .then((reservationIdDto: ReservationIdDto) => {
              let reservationId = reservationIdDto.reservationId;
              localStorage.setItem("ReservationId", reservationId);
              this.rekeningInput = false;
              this.$router.push(`/rekening/${reservationId}`);
            })
            .catch((err: ApiException) => {
              this.$toast.error(err.response);
            })
        return;
      }

      this.loading = true;
      new UserClient(null, this.http).loginWithReservationCode(this.code)
          .then((tokenDto: JwtTokenDto) => {
            let token = tokenDto.token;
            localStorage.setItem("Token", token);
            localStorage.setItem("Code", this.code);
            store.commit('logIn', token);
            this.$router.push("/overview");
          })
          .catch((err: ApiException) => {
            this.invalidCode = true;
            this.$toast.error(err.response);
          })
          .finally(() => {
            this.loading = false;
          })
    }
  },
  mounted(){
    if (this.$router.currentRoute.value.fullPath == "/rekeningInput"){
      this.rekeningInput = true;
    } else {
      this.rekeningInput = false;
    }
  }
})
</script>

<style scoped lang="scss">
@import '@/assets/css/colors.scss';
  .toggleLoginViews {
    font-weight: bold;
    color: grey;
    text-decoration: underline;
    cursor: pointer;
    font-size: small;

    &:hover {
      color: rgb(157, 157, 157);
    }
  }

  .invalidMessage {
    color: red;
  }

  main {
    width: 100vw;
    height: calc(100vh - 60px);
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 25px;

    > h1 {
      font-size: 40px;
    }

    > * {
      margin: 0 !important;
    }
}

  .inputField {
  background-color: transparent;
  outline: solid 3px $orange;
  width: 300px;
  height: 60px;
  border-radius: 15px;
  margin-bottom: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.inputField > input {
  border: none;
  background-color: transparent;
  width: 95%;
  height: 90%;
  font-size: 25px;
  font-weight: bold;
  color: $dark;
  text-align: center;
}

input:focus {
  outline: none;
}

button {
  font-family: Inter;
  background-color: $dark;
  color: $creme;
  padding: 10px 20px;
  border-radius: 15px;
  font-size: 25px;
  width: 300px;
  height: 55px;
  border: none;
}

button:hover {
  cursor: pointer;
  background-image: linear-gradient(rgba(255, 255, 255, 0.15) 0 0);
}
</style>