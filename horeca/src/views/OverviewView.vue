<template>
  <loader v-if="loading" />
  <div v-if="modal" class="modalContainer">
    <modal :prevent-close="false" title="Bestellen?" @close="closeModal">
      <div class="modalConfirmation" >
        <div v-for="order in orders">
          <img :src="`${axios.defaults.baseURL}/FileStorage/${getProductById(order.productId).name}.png?version=${Math.random()}`"><h4>{{ order.amount }}x {{ getProductById(order.productId).name }}: {{ formatPrice(order.amount * getProductById(order.productId).price) }},- </h4>
        </div>
        <h3>Totaal: {{ formatPrice(getTotalPrice()) }},-</h3>
        <button class="orderButton" @click="confirmOrder">Bestel</button>
      </div>
    </modal>
  </div>
  <main>
    <section class="left">
      <h1 v-if="loggedInWithCode">Bestel hier je drankjes en hapjes</h1>
      <h1 v-else style="color: red">Log in met je reserveringscode om te bestellen</h1>
      <div v-for="(product, category) in products" class="category" :class="`category${category.replace(' ', '_')}`">
        <h2 @click="handleCollapse(`category${category.replace(' ', '_')}`)">{{ category }} <img class="categoryArrow" src="@/assets/img/icon/arrow-right.svg" alt=""></h2>
        <div class="items collapsed">
          <product-card v-for="singleProduct in product" :product="singleProduct" @click="addProduct(singleProduct.id)" />
        </div>
      </div>
    </section>
    <section class="right">
      <div class="content">
        <h2>Bestelling</h2>
        <div class="orders">
          <h3 v-for="order in orders">{{ order.amount }}x {{ getProductById(order.productId).name }}: {{ formatPrice(order.amount * getProductById(order.productId).price) }},- <button @click="removeProduct(order.productId)">X</button></h3>
          <!-- we do this loop below to add empty lines -->
          <h3 v-for="i in 15 - orders.length"></h3>
          <h3>Totaal: {{ formatPrice(getTotalPrice()) }},-</h3>
        </div>
        <button class="orderButton" @click="openModal">Plaats bestelling</button>
      </div>
    </section>
  </main>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import modal from '../components/modal.vue';
import productCard from "@/components/productCard.vue";
import { OrderClient, ProductClient } from '!/ApiClient';
import type { Product, ProductOrderModel } from '!/ApiClient';
import axios from "axios";
import loader from "@/components/loader.vue";
import { formatPrice } from '@/store/functions';

export default defineComponent({
  components: { modal, productCard, loader },
  data() {
    return {
      loading: false,
      modal: false,
      products: {} as { [category: string]: Product[] },
      orders: [] as ProductOrderModel[],
      axios: axios,
      loggedInWithCode: localStorage.getItem("Code") != null
    }
  },
  methods: {
    getProductById(productId: number): Product {
      for (let category in this.products) {
        let products = this.products[category];
        for (let product of products) {
          if (product.id == productId)
            return product;
        }
      }

      return {} as Product;
    },
    addProduct(productId: number): void {
      let order = this.orders.find((order) => order.productId == productId);
      if (order == null) {
        order = { productId: productId, amount: 0 } as ProductOrderModel;
        this.orders.push(order);
      }
      order.amount += 1
    },
    getTotalPrice(): number {
      let price = 0;
      const getProductById = this.getProductById;
      this.orders.forEach(function(productOrder: ProductOrderModel) {
        price += productOrder.amount * getProductById(productOrder.productId).price;
      })
      return price;
    },
    removeProduct(productId: number): void {
      let order: ProductOrderModel | undefined = this.orders.find((order) => order.productId == productId);
      if (order == undefined) return;
      order.amount -= 1;
      if (order.amount == 0) {
        this.orders.splice(this.orders.indexOf(order), 1);
      }
    },
    openModal(): void {
      if (this.checkIfOrderIsValid()){
        return
      }
      this.modal = true
    },
    checkIfOrderIsValid(){
      if (this.orders.length == 0) {
        this.$toast.info(`Je moet minimaal 1 product kiezen`)
        return true
      }
      return false
    },
    closeModal(): void {
      this.modal = false
    },
    confirmOrder(): void {
      if (!this.loggedInWithCode) {
        this.$toast.error("Je moet ingelogd zijn met een reservering code om te kunnen bestellen!");
        return;
      }
      this.loading = true;
      //@ts-ignore
      new OrderClient(null, this.http).add(localStorage.getItem("Code")!, this.orders)
          .then(() => {
            this.$toast.success("Je bestelling is bevestigd en word afgehandeld.");
            this.orders = [];
            this.modal = false;
          })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
          .finally(() => {
            this.loading = false;
          });
    },
    handleCollapse(reference:string){
      const element = document.querySelector(`.${reference}`)
      const arrowElement = element?.querySelector(".categoryArrow")
      const childElement = element?.querySelector(".items")
      arrowElement?.classList.toggle("turnedArrow")
      childElement?.classList.toggle("collapsed")
    },
    formatPrice
  },
  mounted() {
    this.loading = true;
    //@ts-ignore
    new ProductClient(null, this.http).getProducts()
        .then((products) => {
          this.products = products;
        })
        .catch((err: ApiException) => {
          this.$toast.error(err.response);
        })
        .finally(() => {
          this.loading = false;
        });
  }
})
</script>

<style scoped lang="scss">
@import '@/assets/css/colors.scss';

main {
  width: 90%;
  margin-left: 5%;
  display: flex;
  justify-content: space-between;
}

.orders {
  height: 70%;
  width: 90%;
}

.left {
  width: 67.5%;

  > h1 {
    text-align: end;
    margin-bottom: 140px;
  }
}

.right {
  width: 27.5%;
  background-color: rgba(210, 174, 57, 0.1);
  height: calc(100vh - 60px);


  > .content {
    width: calc(100% - 50px);
    height: calc(100% - 40px);
    margin-left: 25px;
    margin-top: 20px;
    display: flex;
    justify-content: space-between;
    align-items: start;
    flex-direction: column;
  }
}

.orderButton {
  color: black;
  border-radius: 15px;
  font-size: 20px;
  background-color: $orange;
  padding: 15px 30px;
  border: none;
  font-weight: bold;

  &:hover {
    cursor: pointer;
    background-image: linear-gradient(rgba(0, 0, 0, 0.15) 0 0);
  }
}

.items {
  display: flex;
  flex-wrap: wrap;
  gap: 30px;
}

$notesColor: #f1e8a8;

.orders {
  overflow-y: scroll;

  &::-webkit-scrollbar {
    width: 5px;
  }

  /* Track */
  &::-webkit-scrollbar-track {
    background: transparent;
  }

  /* Handle */
  &::-webkit-scrollbar-thumb {
    background: $notesColor;
  }

  /* Handle on hover */
  &::-webkit-scrollbar-thumb:hover {
    background: rgb(223, 217, 166);
  }

  > h3 {
    border-bottom: solid 2px $notesColor;

    position: relative;
    > button {
      position: absolute;
      right: 0;
      font-family: Inter;
      font-size: bolder;
      color: $creme;
      background-color: $red;
      border: none;
      font-size: 18px;
      border-radius: 5px;
      top: -3px;

      &:hover {
        cursor: pointer;
        background-image: linear-gradient(rgba(0, 0, 0, 0.15) 0 0);
      }
    }
  }
}

.category{
  h2{
    cursor: pointer;
    img{
      width: 20px;
      height: 20px;
    }
  }
}

.modalConfirmation {
  padding: 1rem 4rem 4rem 2rem;
  min-width: 400px;
  height: auto;
  text-align: center;

  > div {
    display: flex;
    justify-content: center;
    align-items: center;

    > img {
      height: 40px;
      object-fit: cover;
    }
  }
}

.collapsed{
  display: none !important;
}
.turnedArrow{
  transform: rotate(90deg);
}
</style>