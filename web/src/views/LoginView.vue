<template>
  <Loader v-if="showLoader" />
  <main>
    <div class="login">
      <div class="loginMain"> 
        <h1 v-if="register">Registreer</h1>
        <h1 v-else>Login</h1>
        <div v-if="register" class="inputField">
          <input type="text" v-model="form.name" placeholder="Naam">
        </div>
        <div class="inputField">
          <input type="text" v-model="form.email" placeholder="Email">
        </div>
        <div class="inputField" :class="passwordTooShort ? 'removeBottomMargin' : ''">
          <input type="password" @keyup.enter="onLogin" v-model="form.password" placeholder="Wachtwoord">
        </div>
        <p class="passwordTooShort" v-if="passwordTooShort">Wachtwoord moet minimaal 4 characters zijn!</p>
        <div v-if="register" class="inputField" :class="[form.password == form.confirmPassword ? '' : 'invalidField']">
          <input type="password" v-model="form.confirmPassword" placeholder="Bevestig wachtwoord">
        </div>
        <div v-if="register" class="inputField">
          <input type="text" v-model="form.phoneNumber" placeholder="Telefoonnummer">
        </div>
        <div v-if="register" class="inputField">
          <input type="text" v-model="form.streetName" placeholder="Adres">
        </div>
        <div v-if="register" class="inputField">
          <input type="text" v-model="form.town" placeholder="Woonplaats">
        </div>
        <div v-if="register" class="inputField">
          <input type="text" v-model="form.zipCode" placeholder="Postcode">
        </div>
        <div class="buttons">
          <button @click="onLogin" :class="[!register ? 'active' : 'unactive', !register ? 'order1' : 'order2']">Log In</button>
          <button @click="onRegister" :disabled="register ? (form.password == form.confirmPassword ? false : true) : false" :class="[register ? 'active' : 'unactive', register ? 'order1' : 'order2']">Registreer</button>
        </div>
        <p @click="forgotPassword" v-if="!register" class="forgotPassword">Wachtwoord vergeten</p>
      </div>
    </div>
    <div class="image">

    </div>
  </main>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { store } from '@/store/store';
//@ts-ignore
import { UserClient, JwtTokenDto, ApiException } from '!/ApiClient';
import Loader from "@/components/Loader.vue";

export default defineComponent({
  components: {Loader},
  data() {
    return {
      showLoader: false,
      register: true,
      passwordTooShort: false,
      form: {
        name: "",
        email: "",
        password: "",
        confirmPassword: "",
        streetName: "",
        town: "",
        zipCode: "",
        phoneNumber: "",
      }
    }
  },
  methods: {
    //hier alle functies / methodes
    updateUrl() {
      this.$router.push(this.register ? "/registreer" : "/login")
    },
    forgotPassword() {
      //@ts-ignore
      new UserClient(null, this.http).forgotPassword(this.form.email).then(() => {
        this.$toast.success("Wachtwoord reset link verstuurd!");
      }).catch((err: ApiException) => {
        this.$toast.error(err.response);
      }); 
    },
    onLogin() {
      if (!this.register) {
        this.showLoader = true;
        //@ts-ignore
        new UserClient(null, this.http).login({
          email: this.form.email,
          password: this.form.password
        }).then((tokenDto: JwtTokenDto) => {
          let token = tokenDto.token;
          localStorage.setItem("Token", token)
          store.commit('logIn', token);
          this.$router.push("/");
          this.$toast.success("Ingelogd");
        })
        .catch((err: ApiException) => {
          this.showLoader = false;
          this.$toast.error(err.response);
        })
      } else {
        this.register = !this.register;
        this.updateUrl();
      }
    },
    onRegister() {
      if (this.register) {
        if (this.form.password.length < 4) {
          this.passwordTooShort = true;
          return;
        } else {
          this.passwordTooShort = false;
        }
        this.showLoader = true;
        //@ts-ignore
        new UserClient(null, this.http).register({
          name: this.form.name,
          city: this.form.town,
          email: this.form.email,
          password: this.form.password,
          phoneNumber: this.form.phoneNumber,
          streetName: this.form.streetName,
          zipCode: this.form.zipCode
        })
        .then(() => {
          this.showLoader = false;
          this.register = !this.register;
          this.$router.push("/login");
          this.$toast.success("Account aangemaakt");
        }).catch((msg: ApiException) => {
          this.showLoader = false
          this.$toast.error(msg.response);
        })
        // this.$router.push("/login")
      } else {
        this.register = !this.register;
        this.updateUrl();
      }
    }
  },
  mounted() {
    //dit runt elke keer bij start.
    this.register = this.$route.path == "/registreer";
  },
})
</script>

<style scoped lang="scss">
@import '@/assets/css/colors.scss';

.forgotPassword {
  color: grey;
  text-decoration: underline;
  cursor: pointer;

  &:hover {
    color: rgb(88, 85, 85);
  }
}

.removeBottomMargin {
  margin-bottom: 0 !important;
}

.passwordTooShort {
  margin-top: 0;
  color: red;
}

.buttons {
  display: flex;
  justify-content: space-between;
}

.buttons > button[disabled] {
  cursor: not-allowed !important;
  background-image: linear-gradient(rgba(225, 225, 225, 0.3) 0 0) !important;
}


.order1 {
  order: 1;
}

.order2 {
  order: 2;
}

.buttons > button {
  height: 50px;
  width: 40%;
  border-radius: 15px;
  border: none;
  color: $creme;
  font-size: 20px;
}

.buttons > button:hover {
  cursor: pointer;
  background-image: linear-gradient(rgb(0 0 0/20%) 0 0);
}

.active {
  background-color: $orange-2;
}

.unactive {
  background-color: $orange-3;
}

main {
  position: relative;
}

.login {
  background-color: $orange;
  height: 800px;
  width: 40%;
  display: flex;
  justify-content: center;
  align-items: center;
}

.loginMain {
  width: 70%;
  height: auto;
}

.inputField {
  background-color: $creme;
  width: 100%;
  height: 55px;
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
  font-size: 20px;
  font-weight: bold;
  color: $dark;
}

.inputField > input:focus {
    outline: none;
}

.inputField:focus-within {
  outline: solid $dark 2px;
}

.invalidField {
  outline: solid $red 2px !important;
  background-image: linear-gradient(rgba(255, 0, 0, 0.2) 0 0);
}

.image {
  background-image: url('@/assets/login.png');
  height: 800px;
  width: 200%;
  position: absolute;
  top: 0;
  left: 40%;
  background-size: contain;
}

@media only screen and (max-width: 750px) {
  .image {
    display: none;
  }

  .login {
    width: 100%;
  }
}
@media only screen and (max-height: 120000px) and (min-height: 1200px) {

}
</style>