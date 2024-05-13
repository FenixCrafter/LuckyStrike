<template>
  <main>
    <Loader v-if="loading"/>
    <div>
      <h1>Registreer Werknemer</h1>
      <div class="inputField">
        <input type="text" v-model="form.naam" placeholder="naam">
      </div>
      <div class="inputField">
        <input type="text" v-model="form.email" placeholder="email">
      </div>
      <div class="inputField">
        <input type="password" v-model="form.password" placeholder="wachtwoord">
      </div>
      <button @click="onRegister">Registreer</button>
    </div>
    <div class="overview" v-if="!loading">
      <table v-if="employees.length > 0">
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Acties</th>
        </tr>
        <tr v-for="employee in employees">
          <td>{{ employee.fullName }}</td>
          <td>{{ employee.email }}</td>
          <td><button @click="deleteEmployee(employee.id!)">Verwijder</button></td>
        </tr>
      </table>
      <h3 v-else>Geen werknemers geregistreerd</h3>
    </div>
  </main>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
//@ts-ignore
import {UserClient, RegisterModel} from '!/ApiClient';
import type { User } from '!/ApiClient';
import { roleMatch } from '@/store/auth-functions';
import {Roles} from "@/enums/Roles";
import Loader from "@/components/Loader.vue";

export default defineComponent({
  components: {Loader},
  data() {
    return {
      employees: [] as User[],
      form: {
        naam: "",
        email: "",
        password: "",
      },
      loading: false,
    }
  },
  methods: {
    fetchEmployees() {
      console.log(this.loading)
      this.loading = true
      console.log(this.loading)
      new UserClient(null, this.http).getEmployees()
          .then((users: User[]) => {
            this.employees = users;
            this.loading = false
          })
    },
    deleteEmployee(id: string) {
      this.loading = true
      new UserClient(null, this.http).deleteUser(id)
          .then(() => {
            this.$toast.success("Werknemer verwijderd!");
            this.fetchEmployees();
            this.loading = false
          })
          .catch(() => {
            this.$toast.error("Werknemer niet verwijderd!");
            this.loading = false
          })
    },
    onRegister() {
      this.loading = true
      //@ts-ignore
      new UserClient(null, this.http).registerEmployee({
        name: this.form.naam,
        email: this.form.email,
        password: this.form.password
      } as RegisterModel)
          .then(() => {
            this.$toast.success("Medewerker is geregistreerd")
            this.form.naam = "";
            this.form.email = "";
            this.form.password = "";
            this.fetchEmployees();
            this.loading = false
          }).catch((msg: any) => {
        this.$toast.error("Medewerker is niet geregistreerd")
        this.loading = false
      })
    }
  },
  mounted() {
    // this.loading = true
    this.fetchEmployees();
    if (localStorage.getItem("Token") == null || !roleMatch([Roles.ADMIN])) {
      this.$router.push("/")
      this.$toast.error("Je hebt geen toegang tot deze pagina")
    }
    // this.loading = false

  }
})
</script>

<style scoped lang="scss">
@import '@/assets/css/colors.scss';

main {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  justify-content: flex-start;
  text-align: center;
  width: 50%;
  margin-left: 25%;
  gap: 25px;
  margin-bottom: 20px;
}

.overview {
  > table {
    > tr {
        outline: solid 1px black;
      > td {
        padding: 10px 20px;

        > button {
          width: auto;
          height: auto;
          padding: 10px 20px;
        }
      }
    }
  }
}

.inputField {
  background-color: $orange;
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

button {
  height: 50px;
  width: 40%;
  border-radius: 15px;
  border: none;
  color: white;
  font-size: 20px;
  background-color: $orange-2;
}

button:hover {
  cursor: pointer;
  background-image: linear-gradient(rgb(0 0 0/20%) 0 0);
}

@media only screen and (max-width: 750px) {
  main {
    width: 80%;
    margin-left: 10%;
  }
}
</style>