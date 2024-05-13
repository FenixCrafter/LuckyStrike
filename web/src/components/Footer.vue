<template xmlns="http://www.w3.org/1999/html">
  <div class="footer">
    <div class="component1">
      <div class="logo">
        <img alt="logo" src="@/assets/img/luckystrike-logo.png"/>
      </div>
      <div class="address">
        <h1 class="Title">Bowlingbaan lucky strike</h1>
        <address>
          graafschapcollegstraat 1<br>
          1122BD Doetinchem
        </address>
      </div>
    </div>
    <div class="component2">
      <div class="openingsTijden">
        <h1 class="Title">Openingstijden</h1>
        <p>
          <span>Maandag</span> 14:00 - 22:00
        </p>
        <p>
          <span>Dinsdag</span> 14:00 - 22:00
        </p>
        <p>
          <span>Woensdag</span> 14:00 - 22:00
        </p>
        <p>
          <span>Donderdag</span> 14:00 - 22:00
        </p>
        <p>
          <span>Vrijdag</span> 14:00 - 22:00
        </p>
        <p>
          <span>Zaterdag</span> 14:00 - 24:00
        </p>
        <p>
          <span>Zondag</span> 14:00 - 24:00
        </p>
      </div>
      <div class="scrollToTop" @click="scrollUp">
        <button >

          Terug naar boven

        </button>
        <img src="@/assets/arrow-down.svg" alt="^">

      </div>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue'

export default defineComponent({
  name: "Footer",
  methods: {
    scrollUp() {
      window.scrollTo({
        top: 0,
        behavior: "smooth"
      })
    },
  },
})
</script>

<style lang="scss" scoped>
@import "@/assets/css/colors.scss";

.footer {
  width: 100%;
  height: 350px;
  background: $dark-2;
  position: relative;
  bottom: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-direction: column;
  flex-wrap: wrap;
  color: $creme;
}

.component1 {
  width: 41%;
  //min-height: 100%;
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  //align-items: center;
  margin: 20px 0;
}

.component2 {
  width: 41%;
  //min-height: 100%;
  display: flex;
  flex-direction: row;
  //justify-content: spa;
  align-items: center;
  margin: 20px 0;
}

.logo {
  width: 40%;
  height: 60%;

  img {
    width: 75%;
    height: 150%;
  }
}

.Title {
  color: $orange;
  font-size: 30px;
  font-weight: bold;
}

.openingsTijden {
  text-align: center;
  font-size: 20px;

  p {
    margin: 0;
    padding: 0;

    span {
      width: 110px;
      display: inline-block;
      text-align: left;
    }
  }
}

.scrollToTop {
  transform: rotate(-0.25turn);
  text-align: center;
  position: relative;
  right: 8%;
  bottom: -7%;

  button {
    background-color: $dark-2;
    color: $creme;
    border: none;
    border-radius: 5px;
    padding: 10px;
    font-size: 20px;
    font-weight: bold;
    text-align: center;
    line-height: 20px;
    position: relative;
    top: -12px;
  }

  img {
    padding: 10px;
    width: 15px;
    height: 15px;
    transform: rotate(-0.25turn);
    background: $darkBlueGreen;
  }
}
// Media queries
@media screen and (max-width: 1200px){
 .Title{
   font-size: 20px;
 }
  .openingsTijden{
    p{
      font-size: 15px;
    }
  }
}

.scrollToTop{
  button{
    font-size: 15px;
  }
}

@media only screen and (max-width: 980px) {
  .footer{
    height: 500px;
    flex-direction: row;
  }

  .scrollToTop {
    display: none;
  }

  .component1 {
    width: 83%;
    height: 50%;
    flex-direction: row;
    justify-content: space-evenly;
    align-items: center;
    margin: 0;

    .logo{
      width: 30%;
      height: 100%;
    }

    address{
      padding-left: 11px;
    }
  }

  .component2 {
    width: 100%;
    height: 42%;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    //margin: 20px 0;
  }

  .logo {
    width: 70%;
    height: 50%;
    padding-top: 10px;

    img {
      width: 90%;
      height: 100%;
    }
  }

  .Title {
    padding-left: 10px;
    color: $orange;
    font-size: 30px;
    font-weight: bold;
  }

  .openingsTijden {
    text-align: center;
    font-size: 20px;

    p {
      margin: 0;
      padding: 0;

      span {
        width: 110px;
        display: inline-block;
        text-align: left;
      }
    }
  }
}

@media only screen and (max-width: 920px){
.logo{
  display: none;
}
  .component1, .component2{
    height: 30%;
    width: 100%;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
    margin: 20px;

    .address{
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }
  }
}
</style>
