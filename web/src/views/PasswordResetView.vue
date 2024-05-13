<template>
  <Loader v-if="loading"></loader>
  <main>
    <h1>Reset uw wachtwoord</h1>
    <div class="inputField" :class="passwordTooShort ? 'removeBottomMargin' : ''">
      <input type="password" @keyup.enter="resetPassword" v-model="password" placeholder="Wachtwoord">
    </div>
    <h5 class="passwordTooShort" v-if="passwordTooShort">Wachtwoord moet minimaal 4 characters zijn!</h5>
    <button @click="resetPassword">Reset wachtwoord</button>
  </main>
</template>

<script lang="ts">
  import { NewPasswordModel, UserClient } from '!/ApiClient';
  import loader from '@/components/Loader.vue';

  export default {
    components: { loader },
    data() {
      return {
        loading: false,
        password: "",
        passwordTooShort: false,
      };
    },
    methods: {
      resetPassword() {
        if (this.password.length < 4) {
          this.passwordTooShort = true;
          return;
        } else {
          this.passwordTooShort = false;
        }
        if (this.loading)
          return;
        this.loading = true;
        // @ts-ignore
        new UserClient(null, this.http).newPassword(new NewPasswordModel({
          password: this.password,
          token: this.$route.params.token as string
        }))
        .then(() => {
          this.$toast.success("Wachtwoord aangepast!");
          this.$router.push("/");
        })
        .catch(() => {
          this.$toast.error("Er is iets misgegaan tijdens het aanpassen van uw wachtwoord.")
        })
        .finally(() => {
          this.loading = false;
        });
      }
    }
  };
</script>

<style lang="scss" scoped>
  @import '@/assets/css/colors.scss';

  main {
    text-align: center;
  }

  .inputField > input {
    border: none;
    background-color: $orange;
    width: 250px;
    height: 50px;
    font-size: 20px;
    border-radius: 15px;
    font-weight: bold;
    color: $dark;
    margin-bottom: 20px;
    padding: 5px 10px;
    display: inline-block;
  }

  .passwordTooShort {
    margin-top: 0;
    color: red;
  }

  button {
    height: 50px;
    width: 250px;
    border-radius: 15px;
    border: none;
    color: $creme;
    font-size: 20px;
    background-color: $orange-2;
  }

  button:hover {
    cursor: pointer;
    background-image: linear-gradient(rgb(0 0 0/20%) 0 0);
  }
</style>