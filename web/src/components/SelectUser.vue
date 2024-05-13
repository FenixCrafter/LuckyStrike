<template>
  <Loader v-if="loading"/>
  <!--  Select User-->
  <div v-if="!createUser && !loading" class="modal-content-container users">

    <!--     Title-->
    <div class="selectUserTitle"><img class="returnBtn" @click="$emit('showOverview')"
                                      src="@/assets/img/icon/arrow-right.svg" alt="">
      <h2>Selecteer gebruiker</h2>
    </div>

    <!--    Searchbar-->
    <div class="searchBar">
      <input class="searchUser " type="text" placeholder="Zoek gebruiker" v-model="userQuery">
      <div class="searchBtn maakReserveringBtn" @click="searchUser">Zoek</div>
    </div>

    <!--    Userlist-->
    <Loader v-if="userLoading"/>
    <div v-if="!userLoading" class="gebruikers">

<!--      User-->
      <div v-if="userList.length !== 0" v-for="user in userList" @click="$emit('updateSelectedUser', user)"
           class="gebruiker">
        <div class="userCircle">{{ user.fullName.charAt(0) }}</div>
        <div>
          <p v-if="user.fullName.length <= 23">{{ user.fullName }}</p>
          <p v-else>{{ user.fullName.substring(0, 18) }}...</p>
          <p v-if="user.email.length <= 23">{{ user.email }}</p>
          <p v-else>{{ user.email.substring(0, 18) }}...</p>
        </div>
      </div>
      <div v-else>
        <div class="gebruiker">
          <p>Geen gebruikers gevonden</p>
        </div>
      </div>
    </div>

    <!--    Buttons-->
    <div class="createUserForEmployee">
      <div class="maakReserveringBtn" @click="createUser = true; selectedUser = {}">Maak gebruiker</div>
    </div>
  </div>

<!--  Create User-->
  <div v-if="createUser && !loading" class="modal-content-container createUser">

    <!--    Title-->
    <div class="selectUserTitle"><img class="returnBtn" @click="createUser=false"
                                      src="@/assets/img/icon/arrow-right.svg" alt="">
      <h2>Maak gebruiker aan</h2>
    </div>

    <!--    Fields-->
    <div class="inputField">
      <input type="text" v-model="selectedUser.userName" placeholder="Naam">
    </div>
    <div class="inputField">
      <input type="text" v-model="selectedUser.email" placeholder="Email">
    </div>
    <div class="inputField">
      <input type="text" v-model="selectedUser.phoneNumber" placeholder="Telefoonnummer">
    </div>

    <!--    Button-->
    <div class="maakReserveringBtn" @click="createPartialUser">Maak gebruiker aan</div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import Loader from "@/components/Loader.vue";
import type {User} from "@/store/types";
import {UserClient, ApiException} from "../../../ApiClient";

export default defineComponent({
  components: {Loader},
  props: {
    selectedUserProp: {},
  },
  data() {
    return {
      userQuery: "",
      userList: [] as User[],
      createUser: false,
      loading: false,
      selectedUser: {} as User,
      userLoading: false,
    }
  },
  methods: {

    //searches user based on the query
    searchUser() {
      this.userLoading = true
      //@ts-ignore
      let userClient = new UserClient(null as any, this.http)
      userClient.searchUser(this.userQuery).then((response) => {
        this.userList = response as User[];
      }).catch((msg: ApiException) => {
        this.$toast.error(msg.response);
      })
      this.userLoading = false
    },

    resetSearchUser() {
      this.userQuery = "";
      this.userList = [] as User[];
    },

    //creates a PartialUser which only contains name, email and phonenumber
    createPartialUser() {
      if (this.selectedUser.userName != undefined && this.selectedUser.email != undefined) {
        this.loading = true
        //@ts-ignore
        let userClient = new UserClient(null as any, this.http)
        //@ts-ignore
        userClient.createPartialUser(this.selectedUser).then((response) => {
          this.$toast.success("Gebruiker aangemaakt")
          this.$emit('updateSelectedUser', response)
          this.resetSearchUser()
          this.loading = false
        }).catch((msg: ApiException) => {
          this.$toast.error(msg.response);
        })
      } else {
        this.$toast.info("Je moet eerst een naam en email invoeren")
      }
    },
  }
})
</script>


<style scoped lang="scss">
@import "../assets/css/colors";

.disable {
  display: none;
}

.hero-booking-container {
  width: 900px;
  height: 90px;
  margin-bottom: 10rem;
  border-radius: 28px;
  background: $creme;
  display: flex;
  justify-content: space-evenly;
  align-items: center;
  //overflow: auto;
}

.field {
  display: flex;
  height: 50%;
  width: 15%;
  background: $orange-4;
  justify-content: space-between;
  cursor: pointer;

  &-content {
    display: flex;
    padding-left: 1rem;
    align-items: center;
    width: 70%;

    img {
      height: 50%;
      color: $orange-5;
      margin-right: .5rem;
    }

    p {
      color: $creme;
    }
  }

  &-action {
    width: 50px;
    background: $orange-5;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;

    img {
      width: 50%;
      filter: invert(1);
    }
  }
}

.dateField {
  width: 25%;

  .field-content {
    background: $orange-4;
  }
}

.dp__theme_light {
  --dp-background-color: $orange-4;
  --dp-text-color: #212121;
  --dp-hover-color: $orange-4;
  --dp-hover-text-color: #212121;
  --dp-hover-icon-color: #959595;
  --dp-primary-color: #1976d2;
  --dp-primary-disabled-color: #6bacea;
  --dp-primary-text-color: #f8f5f5;
  --dp-secondary-color: #c0c4cc;
  --dp-border-color: $orange-4;
  --dp-menu-border-color: #ddd;
  --dp-border-color-hover: #aaaeb7;
  --dp-disabled-color: #f6f6f6;
  --dp-scroll-bar-background: #f3f3f3;
  --dp-scroll-bar-color: #959595;
  --dp-success-color: #76d275;
  --dp-success-color-disabled: #a3d9b1;
  --dp-icon-color: #959595;
  --dp-danger-color: #ff6f60;
  --dp-marker-color: #ff6f60;
  --dp-tooltip-color: #fafafa;
  --dp-disabled-color-text: #8e8e8e;
  --dp-highlight-color: rgb(25 118 210 / 10%);
  --dp-range-between-dates-background-color: var(--dp-hover-color, #f3f3f3);
  --dp-range-between-dates-text-color: var(--dp-hover-text-color, #212121);
  --dp-range-between-border-color: var(--dp-hover-color, #f3f3f3);
}

.reserverenBtn {
  background: $dark;
  width: 25%;

  .field-action {
    background: $gray-2;
    transition: transform 0.2s ease-in;
    cursor: pointer;

    img {
      filter: invert(1);
    }
  }
}

.banen {
  width: 15%;
}

.dropdown-content {
  padding: .5rem 1rem 0 1rem;
  height: 70%;
  width: 50%;
  z-index: 1000;

  .dropdown-field {
    width: 100%;
    height: 50%;
    display: flex;
    flex-direction: column;
    justify-content: space-around;

    &-title {
      margin: 0;
    }

    &-actions {
      width: 80%;
      height: 50%;
      //background: yellow;
      display: flex;
      flex-direction: row;
      background: $orange-4;

      .increase, .decrease {
        background: $orange-5;
        width: 25%;
        display: flex;
        justify-content: center;
        align-items: center;

        img {
          width: 80%;
          filter: invert(1);
        }
      }

      .decrease {
        img {
          transform: rotate(180deg);
        }
      }

      .value {
        width: 50%;
        display: flex;
        justify-content: center;
        align-items: center;

        input {
          width: 100%;
          height: 80%;
          //color: $creme;
          margin: 0;
        }
      }
    }
  }
}

.modal-content-container {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;

  .information {
    width: 80%;
    margin: 0 auto;
    text-align: center;
  }

  .selectLaneContainer {
    margin-top: 2rem;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: row;
  }

  div {

    .modalTable {
      margin-right: 1rem;

      tr {
        height: 3rem;
        width: 3rem;
        display: flex;
        justify-content: center;
        align-items: center;

        td {
          width: 100%;
          height: 100%;
          display: flex;
          justify-content: center;
          align-items: center;

          .avaiablelity {
            width: 80%;
            height: 80%;
            background-color: $green;
            border-radius: 10px;
          }

          .selected {
            border: 2px solid $dark;
            background-image: url("@/assets/img/icon/check-mark-svgrepo-com.svg") !important;
            background-repeat: no-repeat;
            background-size: cover;
          }

          .magicBowlen {
            background-color: $disco;
          }
        }
      }


      .bowlingbalImg {
        color: $creme;
        transform: translateY(-1rem);
        width: 3rem;
        height: 3rem;
        display: flex;
        justify-content: center;
        align-items: center;
        background-image: url("@/assets/img/bowlinbal.png");
        background-repeat: no-repeat;
        background-size: cover;
        background-position: center;
      }
    }
  }
}

.kidLane {
  border: 2px solid silver;
  border-radius: 10px;
}

.maakReserveringBtn {
  margin-top: 2rem;
  max-width: 300px;
  height: auto;
  padding: .8rem;
  border-radius: 10px;
  background: $dark;
  color: $creme;
  cursor: pointer;
}

.notActive {
  display: none;
}

.confirmation {
  justify-content: left;
  align-items: center;

  p {
    text-align: left;
    justify-content: left;
  }

  .selectedLanesAndTimes {
    display: flex;
    align-items: center;
    flex-direction: row;
    margin-bottom: .3rem;

    .laneNumber {
      font-size: 1.5rem;
      font-weight: bold;
      color: $creme;
      width: 3rem;
      height: 3rem;
      margin-right: .5rem;
      display: flex;
      justify-content: center;
      align-items: center;
      background-image: url("@/assets/img/bowlinbal.png");
      background-repeat: no-repeat;
      background-size: cover;
      background-position: center;
    }
  }

  .confirmationButtons {
    display: flex;
    justify-content: center;

    div:first-child {
      margin-right: .5rem;
    }

    div:last-child {
      color: $dark;
      background: none;
      border: 2px solid $dark;
    }
  }
}

.colorBlack {
  color: black;
}

.bezet {
  background: red !important;
}

.dropdown {
  z-index: 999 !important;
}

.selecteerGebruikerBtn {
  text-align: center;
}

.selectUserTitle {
  display: flex;
  flex-direction: row;
  width: 300px;
  align-items: center;
}

.returnBtn {
  width: 20px;
  height: 20px;
  margin-right: 1rem;
  transform: rotateY(180deg);
  cursor: pointer;
}

.searchBar {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
}

.searchUser {
  padding: .7rem;
  border-radius: .5rem;
  height: 35%;
  width: 60%;
  margin: 1rem;
}

.searchBtn {
  margin: 0;
  width: 30%;
  text-align: center;
}

.gebruikers {
  display: flex;
  flex-direction: column;
  //justify-content: center;
  align-items: center;
  overflow-y: auto;
  height: 60%;
  width: 300px;
}

.gebruiker {
  width: 260px;
  height: 60px;
  background: $orange-4;
  display: flex;
  flex-direction: row;
  //justify-content: center;
  align-items: center;
  margin-bottom: 1rem;
  padding: 0 1rem 0 1rem;
  border-radius: .5rem;
  cursor: pointer;

  .userCircle {
    width: 40px;
    height: 40px;
    //background: r;
    background: $gray-3;
    margin-right: 1rem;
    border-radius: 50%;
    justify-content: center;
    align-items: center;
  }

  div {
    display: flex;
    justify-content: center;
    flex-direction: column;
  }

  div:last-child {
    height: 100%;
  }

  p {
    margin: 0;
  }
}

.inputField {
  background-color: $creme;
  width: 30%;
  height: 55px;
  border-radius: 15px;
  margin-bottom: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
  outline: solid $dark 2px;
}

.inputField > input {
  padding: 1rem;
  border: none;
  background-color: transparent;
  width: 100%;
  height: 90%;
  font-size: 20px;
  font-weight: bold;
  //border: 2px solid $dark;
  color: $dark;
}

.inputField > input:focus {
  outline: none;
}

@media screen and (width < 920px) {
  .hero-booking-container {
    flex-direction: column;
    height: 370px;
    margin-bottom: 1% !important;
    width: 40%;
    justify-content: normal;
  }
  .field {
    height: 40px;
    width: 90% !important;
    margin-top: 5%;
  }

}

@media only screen and (max-width: 800px) {
  .hero-booking-container {
    width: 300px;
    height: 320px !important;
  }
  .field {
    height: 40px;
    width: 260px !important;
    margin-top: 5%;
  }
  .disable {
    display: block;
  }
  .information {
    margin-bottom: 0;
    transform: translateY(40%);
  }
  .maakReserveringBtn {
    margin: 1rem 0 0 0;
  }
  .gebruikers {
    width: 90% !important;
  }
  .inputField {
    width: 90% !important;
  }
}

@media only screen and (max-width: 630px) {
  .modalTable {
    margin-right: .1rem !important;
  }
}

@media only screen and (max-width: 630px) {
  .selectLaneContainer {
    transform: scale(0.8);
  }
}

@media only screen and (max-width: 410px) {
  .selectLaneContainer {
    transform: scale(0.7);
  }

  .confirmationButtons {
    flex-direction: column !important;
  }
}


</style>

