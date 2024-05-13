<template>
  <div class="item">
  <img :src="`${axios.defaults.baseURL}/FileStorage/${product.name}.png?version=${Math.random()}`">
    <p>{{ product.name }}: {{ formatPrice(product.price) }},-</p>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import { Product } from "!/ApiClient";
import { formatPrice } from "@/store/functions";
import axios from "axios";

export default defineComponent({
  name: 'productCard',
  data() {
    return { axios: axios }
  },
  props: {
    product: {} as Product
  },
  methods: {
    formatPrice
  }

})
</script>

<style lang="scss">
@import "@/assets/css/colors.scss";
.item {
  user-select: none;
  background-image: url("@/assets/noteit.png");
  width: 200px;
  height: 200px;
  background-size: cover;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  flex-direction: column;
  font-weight: bold;
  transition: scale 0.1s;
  cursor: pointer;

  > p {
    color: $dark-2;
    text-align: center;
  }

  > img {
    height: 50%;
    max-width: 70%;
    transition: rotate 0.2s;
  }

  &:hover {
    scale: 1.025;
  }

  &:hover > img {
    rotate: 10deg;
  }
}

</style>