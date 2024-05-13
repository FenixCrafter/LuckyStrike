<script lang="ts">
import { defineComponent } from 'vue';
import { isOrderOnTime } from '@/store/functions';
import { OrderClient, ProductOrder } from '!/ApiClient';
import type { Order } from '!/ApiClient';
import loader from "@/components/loader.vue";

export default defineComponent({
  components: { loader },
  data() {
    return {
      loading: false,
      order: {} as Order,
      products: {} as { [category: string]: ProductOrder[] }
    }
  },
  methods: {
    markAsDone() {
      this.loading = true;
      //@ts-ignore
      new OrderClient(null, this.http).setOrderReady(this.order.id, true)
      .then(() => {
        this.order.isReady = true;
        this.$toast.success("Bestelling gemarkeerd als klaar!");
        this.$router.push("/management");
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    },
    markAsUndone() {
      this.loading = true;
      //@ts-ignore
      new OrderClient(null, this.http).setOrderReady(this.order.id, false)
      .then(() => {
        this.order.isReady = false;
        this.$toast.warning("Bestelling gemarkeerd als niet klaar.");
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    },
    toggleProductReady(productOrder: ProductOrder) {
      this.loading = true;
      //@ts-ignore
      new OrderClient(null, this.http).setProductOrderReady(productOrder.id, productOrder.isReady)
      .then(() => productOrder.isReady = !productOrder.isReady)
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    },
    allProductsReady() {
      for (let category in this.products) {
        for (let product of this.products[category]) {
          if (!product.isReady)
            return false;
        }
      }

      return true;
    },
    goBack() {
      this.$router.push("/management")
    },
    isOrderOnTime
  },
  mounted() {
    this.loading = true;
    //@ts-ignore
    new OrderClient(null, this.http).get(this.$route.params.id)
    .then((order: Order) => {
      this.order = order;
    })
        .catch((err: ApiException) => {
          this.$toast.error(err.response);
        })
    .finally(() => {
      this.loading = false;
    });
    //@ts-ignore
    new OrderClient(null, this.http).getProducts(this.$route.params.id)
    .then((response) => {
      this.products = response;
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

<template>
  <loader v-if="loading" />
  <main>
    <button @click="goBack">Terug naar bestellingen</button>
    <div class="order" :class="isOrderOnTime(order.timeOrdered) ? 'open' : 'lateOpen'">
        <div class="top">
            <span class="orderNumber">#{{ order.id?.toString().padStart(3, "0") }}</span>
            <div>
              <span v-for="lane in order.reservation?.lanes" class="laneNumber">{{ lane.laneNumber }}</span>
            </div>
            <span class="time">
              <svg class="sandWalkerIcon" width="30" height="33" viewBox="0 0 30 33" xmlns="http://www.w3.org/2000/svg">
                <path d="M8.4963 2.89396C7.85763 3.11954 7.32443 3.63517 7.0549 4.28614C6.92013 4.60196 6.89083 4.7631 6.89669 5.15626C6.90255 6.0715 7.30099 7.70861 7.8049 8.89454C8.44943 10.4027 9.24044 11.5307 11.0861 13.5481C12.5334 15.1272 12.6506 15.3012 12.7854 16.1133C12.8381 16.3969 12.8381 16.6031 12.7854 16.8867C12.6506 17.6988 12.5334 17.8729 11.0861 19.452C9.25802 21.4565 8.46115 22.5779 7.81075 24.0797C7.31271 25.2334 6.90255 26.9285 6.89669 27.8438C6.89083 28.2369 6.92013 28.3981 7.0549 28.7139C7.23068 29.1393 7.63497 29.6356 7.99825 29.8676C8.57833 30.2479 8.32052 30.235 15.1233 30.2156L21.3576 30.1963L21.6975 30.0481C22.1662 29.8354 22.6818 29.3004 22.9221 28.7654C23.0861 28.4045 23.1096 28.282 23.1037 27.8567C23.0979 26.9285 22.6994 25.2979 22.1955 24.1055C21.551 22.5973 20.76 21.4693 18.9143 19.452C17.467 17.8664 17.3498 17.6924 17.2151 16.8867C17.1623 16.6031 17.1623 16.3969 17.2151 16.1133C17.3498 15.3012 17.467 15.1272 18.9143 13.5481C20.76 11.5307 21.551 10.4027 22.1955 8.89454C22.6994 7.71505 23.1037 6.02638 23.1037 5.12404C23.1037 4.76954 23.0686 4.58263 22.9397 4.26681C22.7287 3.74474 22.1838 3.17111 21.6975 2.95197L21.3576 2.80372L15.0881 2.79083C9.22286 2.77794 8.79513 2.78439 8.4963 2.89396ZM15.5744 19.2199C16.7756 20.393 18.4279 22.0945 18.7268 22.4684C19.9279 23.9637 20.5197 25.5299 20.5842 27.3475C20.5959 27.7729 20.5901 27.7986 20.4026 27.9791L20.2151 28.166H15.0002H9.78536L9.59786 27.9791C9.41036 27.7986 9.4045 27.7729 9.41622 27.3475C9.45138 26.3033 9.74435 25.0852 10.1721 24.1699C10.6584 23.1387 11.1916 22.4619 12.7854 20.8635C14.9182 18.7301 14.7424 18.8848 15.0002 18.8848C15.1936 18.8848 15.2815 18.9363 15.5744 19.2199Z" />
              </svg>
              <svg class="refreshIcon" width="34" height="34" viewBox="0 0 34 34" fill="none" xmlns="http://www.w3.org/2000/svg">
                <g clip-path="url(#clip0_15_142)">
                  <path d="M19.2439 3.55945L23.4379 7.8502L19.1471 12.0442" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  <path d="M14.6949 30.3794L10.501 26.0886L14.7917 21.8947" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  <path d="M8.80125 20.7585C8.16213 19.379 7.88286 17.8602 7.98951 16.3436C8.09616 14.8271 8.58524 13.3622 9.41113 12.0858C10.237 10.8094 11.3728 9.76299 12.7125 9.04425C14.0521 8.3255 15.5521 7.95784 17.0723 7.97556L23.4381 7.85017M10.5013 26.0886L16.8671 25.9632C18.3873 25.9809 19.8872 25.6132 21.2269 24.8945C22.5665 24.1758 23.7023 23.1294 24.5282 21.8529C25.3541 20.5765 25.8432 19.1117 25.9498 17.5951C26.0565 16.0786 25.7772 14.5597 25.1381 13.1803" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </g>
                <defs>
                  <clipPath id="clip0_15_142">
                    <rect width="24" height="24" fill="white" transform="translate(0 16.7759) rotate(-44.3465)"/>
                  </clipPath>
                </defs>
              </svg>
              <span>{{ order.timeOrdered?.getHours() }}:{{ order.timeOrdered?.getMinutes().toString().padStart(2, "0") }}</span>
            </span>
            <span class="onTime">
              <svg class="clockIcon" width="41" height="39" viewBox="0 0 41 39" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M20.4998 35.75C29.9347 35.75 37.5832 28.4746 37.5832 19.5C37.5832 10.5254 29.9347 3.25 20.4998 3.25C11.065 3.25 3.4165 10.5254 3.4165 19.5C3.4165 28.4746 11.065 35.75 20.4998 35.75Z" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M20.5 9.75V19.5L27.3333 22.75" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
              <svg class="bellIcon" width="36" height="34" viewBox="0 0 36 34" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M6.41589 2.5652L4.49288 0.435053C0 5.34503 0 8.90345 0 13.5002H1.36345L2.7269 13.4067C2.72749 9.01302 2.72749 6.59477 6.41589 2.5652ZM25.5054 0.435053L23.5853 2.5652C27.2725 6.59477 27.2725 9.01366 27.2725 13.5002L30 13.4067C29.9994 8.90345 29.9994 5.34503 25.5054 0.435053ZM14.9997 30C16.7868 30 18.2809 28.749 18.8416 26.9997H11.1566C11.7174 28.749 13.2115 30 14.9997 30ZM24.545 18.8787V11.9997C24.545 7.17484 21.565 3.10983 17.5286 1.88652C17.129 0.780518 16.1528 0 14.9997 0C13.8454 0 12.8693 0.780518 12.4697 1.88652C8.43206 3.10983 5.45439 7.17484 5.45439 11.9997V18.8787L3.1265 21.4394C2.86928 21.7204 2.7269 22.1014 2.7269 22.4997V23.9995C2.7269 24.8283 3.33685 25.4993 4.09035 25.4993H25.9079C26.6614 25.4993 27.2713 24.8283 27.2713 23.9995V22.4997C27.2713 22.1014 27.1278 21.7204 26.8717 21.4394L24.545 18.8787Z" />
              </svg>
              <label></label>
            </span>
        </div>
        <div v-for="(items, index) in products" class="category" >
          <div class="label">
            <svg width="24" height="24" xmlns="http://www.w3.org/2000/svg" fill-rule="evenodd" clip-rule="evenodd"><path d="M21.883 12l-7.527 6.235.644.765 9-7.521-9-7.479-.645.764 7.529 6.236h-21.884v1h21.883z"/></svg>
            <span>{{ index }}</span>
          </div>
          <div class="items">
              <div v-for="product in items" class="orderitem">
                  <span>{{ product.amount }} {{ product.product.name }}</span>
                  <label :class="product.isReady ? 'finished' : 'unfinished'">{{ product.isReady ? 'Klaar' : 'Niet klaar' }}</label>
                  <button @click="toggleProductReady(product)" :class="product.isReady ? 'unfinished' : 'finished'">{{ product.isReady ? 'Zet op niet klaar' : 'Zet op klaar' }}</button>
              </div>
          </div>
        </div>
        <div class="markAsDone">
            <button undone v-if="!order.isReady" :disabled="!allProductsReady()" @click="markAsDone()">
                <svg width="25" height="25" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path d="M20.285 2L9 13.567L3.714 8.556L0 12.272L9 21L24 5.715L20.285 2Z" />
                </svg>
                <span>Markeer bestelling als klaar</span>
            </button>
            <button done v-else @click="markAsUndone()">
                <svg width="25" height="25" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path d="M20.285 2L9 13.567L3.714 8.556L0 12.272L9 21L24 5.715L20.285 2Z" />
                </svg>
                <span>Markeer bestelling als niet klaar</span>
            </button>
        </div>
    </div>
  </main>
</template>


<style scoped lang="scss">
@import '@/assets/css/colors.scss';

main {
  width: 60vw;
  margin-left: 20vw;

  > button {
    color: black;
    border-radius: 15px;
    font-size: 20px;
    background-color: $orange;
    padding: 15px 30px;
    border: none;
    font-weight: bold;
    margin: 20px 0;

    &:hover {
      cursor: pointer;
      background-image: linear-gradient(rgba(0, 0, 0, 0.15) 0 0);
    }
  }
}

.order {
    width: 100%;
    height: auto;
    background-color: $gray-3;
}

.markAsDone {
    margin-top: 75px;
    padding-bottom: 10px;
    width: 90%;
    margin-left: 5%;
    > button {
        &:hover {
            background-image: linear-gradient(rgb(0 0 0/20%) 0 0);
          > svg {
            rotate: 10deg;
          }
        }
        > svg {
            transition: rotate 0.2s;
        }
        cursor: pointer;
        width: 100%;
        display: flex;
        justify-content: center;
        align-items: center;
        gap: 20px;
        font-size: 25px;
        padding: 10px 0px;
        font-weight: bold;
        color: $creme;
        fill: $creme;
        stroke: $creme;
        &[undone] {
          background-color: $green-3;
        }
        &[done] {
          background-color: $red-3;
        }
        &[disabled] {
          cursor: not-allowed;
          background-image: linear-gradient(rgb(0 0 0/40%) 0 0);
        }
        border: none;
        border-radius: 10px;
    }
}

.category {
    width: calc(100%);

    > .label {
      display: flex;
      padding: 0 10px;
      justify-content: flex-start;
      align-items: center;
      color: $gray-4;
      fill: $gray-4;
      stroke: $gray-4;
      background-color: $gray-5;
      height: 40px;
      width: calc(100% - 20px);
      gap: 15px;
      font-weight: bold;
      font-size: 23px;
    }

    > svg {
        height: 25px;
    }
}

.items {
    background-color: transparent;
    //width: 100%;
    padding: 10px 75px;
    width: calc(100% - 150px);
    font-weight: bold;
    font-size: 22px;

    > .orderitem {
        display: flex;
        justify-content: space-around;
        flex-direction: row;
        align-items: center;
        font-weight: bold;

        &:not(:last-child) {
            margin-bottom: 10px;
        }

        > * {
          width: 33%;
        }

        > label, > button {
            padding: 5px 50px;
            text-align: center;
            border-radius: 10px;
            width: 175px;
        }

        > button {
            border: none;
            font-size: 22px;
            font-weight: bold;
            cursor: pointer;
            width: 300px;

            &:hover {
                background-image: linear-gradient(rgb(0 0 0/10%) 0 0);
            }
        }
    }
}

.top {
    height: 70px;
    font-size: 25px;
    font-weight: normal;
    padding: 10px 30px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    gap: 30px;

    .laneNumber {
        text-align: center;
        display:inline-block;
        width: 40px;
        height: 40px;
        font-weight: bold;
        color: white;
        font-size: 32px;
        border-radius: 50%;
        background-color: $blue;
    }

    > .time, > .onTime {
    display: flex;
    justify-content: center;
    align-items: center;

    > svg {
      height: 25px;
    }
  }
}

.unfinished {
    background-color: $red-2;
    color: $red-3;
}

.finished {
    background-color: $green-2;
    color: $green-3;
}

.inProgress {
  color: $orange-7;
  > .top {
    background-color: $orange-6;
  }
  fill: $orange-7;
  stroke: $orange-7;

  .sandWalkerIcon {
    display: none;
  }

  .refreshIcon {
    display: inline-block;
  }

  .clockIcon {
    display: inline-block;
  }

  .bellIcon {
    display: none;
  }
}

.open {
  color: $gray-2;
  > .top {
    background-color: $gray;
  }
  stroke: $gray-2;
  fill: $gray-2;

  .sandWalkerIcon {
    display: inline-block;
  }

  .refreshIcon {
    display: none;
  }

  .clockIcon {
    display: inline-block;
  }

  .bellIcon {
    display: none;
  }
}

.open, .inProgress {
  .onTime {
    > label::before {
      content: "op tijd";
    }
  }
}

.lateOpen, .lateInProgress {
  .onTime {
    > label::before {
      content: "laat";
    }
  }
}

.lateInProgress {

  color: $red-3;
  > .top {
    background-color: $red-2;
  }
  fill: $red-3;
  stroke: $red-3;

  .sandWalkerIcon {
    display: none;
  }

  .refreshIcon {
    display: inline-block;
  }

  .clockIcon {
    display: none;
  }

  .bellIcon {
    display: inline-block;
  }
}

.lateOpen {
  color: $red-5;
  > .top {
    background-color: $red-4;
  }
  fill: $red-5;
  stroke: $red-5;

  .sandWalkerIcon {
    display: inline-block;
  }

  .refreshIcon {
    display: none;
  }

  .clockIcon {
    display: none;
  }

  .bellIcon {
    display: inline-block;
  }
}
</style>