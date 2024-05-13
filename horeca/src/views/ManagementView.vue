<template>
  <loader v-if="loading" />
  <main>
    <h1>Alle bestellingen</h1>
    <div class="content">
      <section>
        <h2>Open bestellingen/bezig</h2>
        <div class="orders">
          <div v-for="order in getUnreadyOrders()" class="order" :class="(isOrderOnTime(order.timeOrdered) ? 'open' : 'lateOpen')">
            <span class="orderNumber">#{{ order.id.toString().padStart(3, "0") }}</span>
            <div>
              <span v-for="lane in order.reservation.lanes" class="laneNumber">{{ lane.laneNumber }}</span>
            </div>
            <span class="time">
              <svg class="sandWalkerIcon" width="30" height="33" viewBox="0 0 30 33" xmlns="http://www.w3.org/2000/svg">
                <path d="M8.4963 2.89396C7.85763 3.11954 7.32443 3.63517 7.0549 4.28614C6.92013 4.60196 6.89083 4.7631 6.89669 5.15626C6.90255 6.0715 7.30099 7.70861 7.8049 8.89454C8.44943 10.4027 9.24044 11.5307 11.0861 13.5481C12.5334 15.1272 12.6506 15.3012 12.7854 16.1133C12.8381 16.3969 12.8381 16.6031 12.7854 16.8867C12.6506 17.6988 12.5334 17.8729 11.0861 19.452C9.25802 21.4565 8.46115 22.5779 7.81075 24.0797C7.31271 25.2334 6.90255 26.9285 6.89669 27.8438C6.89083 28.2369 6.92013 28.3981 7.0549 28.7139C7.23068 29.1393 7.63497 29.6356 7.99825 29.8676C8.57833 30.2479 8.32052 30.235 15.1233 30.2156L21.3576 30.1963L21.6975 30.0481C22.1662 29.8354 22.6818 29.3004 22.9221 28.7654C23.0861 28.4045 23.1096 28.282 23.1037 27.8567C23.0979 26.9285 22.6994 25.2979 22.1955 24.1055C21.551 22.5973 20.76 21.4693 18.9143 19.452C17.467 17.8664 17.3498 17.6924 17.2151 16.8867C17.1623 16.6031 17.1623 16.3969 17.2151 16.1133C17.3498 15.3012 17.467 15.1272 18.9143 13.5481C20.76 11.5307 21.551 10.4027 22.1955 8.89454C22.6994 7.71505 23.1037 6.02638 23.1037 5.12404C23.1037 4.76954 23.0686 4.58263 22.9397 4.26681C22.7287 3.74474 22.1838 3.17111 21.6975 2.95197L21.3576 2.80372L15.0881 2.79083C9.22286 2.77794 8.79513 2.78439 8.4963 2.89396ZM15.5744 19.2199C16.7756 20.393 18.4279 22.0945 18.7268 22.4684C19.9279 23.9637 20.5197 25.5299 20.5842 27.3475C20.5959 27.7729 20.5901 27.7986 20.4026 27.9791L20.2151 28.166H15.0002H9.78536L9.59786 27.9791C9.41036 27.7986 9.4045 27.7729 9.41622 27.3475C9.45138 26.3033 9.74435 25.0852 10.1721 24.1699C10.6584 23.1387 11.1916 22.4619 12.7854 20.8635C14.9182 18.7301 14.7424 18.8848 15.0002 18.8848C15.1936 18.8848 15.2815 18.9363 15.5744 19.2199Z" />
              </svg>
              <span>{{ order.timeOrdered.getHours() }}:{{ order.timeOrdered.getMinutes().toString().padStart(2, "0") }}</span>
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
            <svg @click="orderInfo(order.id)" class="searchIcon" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M11 19C15.4183 19 19 15.4183 19 11C19 6.58172 15.4183 3 11 3C6.58172 3 3 6.58172 3 11C3 15.4183 6.58172 19 11 19Z" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M20.9999 20.9999L16.6499 16.6499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
        </div>
      </section>
      <section>
        <h2>Klaar om af te leveren</h2>
        <div class="orders">
          <div v-for="order in getReadyOrders()" class="order orderDone">
            <span class="orderNumber">#{{ order.id.toString().padStart(3, "0") }}</span>
            <div>
              <span v-for="lane in order.reservation.lanes" class="laneNumber">{{ lane.laneNumber }}</span>
            </div>
            <span class="time">
              <svg class="sandWalkerIcon" width="30" height="33" viewBox="0 0 30 33" xmlns="http://www.w3.org/2000/svg">
                <path d="M8.4963 2.89396C7.85763 3.11954 7.32443 3.63517 7.0549 4.28614C6.92013 4.60196 6.89083 4.7631 6.89669 5.15626C6.90255 6.0715 7.30099 7.70861 7.8049 8.89454C8.44943 10.4027 9.24044 11.5307 11.0861 13.5481C12.5334 15.1272 12.6506 15.3012 12.7854 16.1133C12.8381 16.3969 12.8381 16.6031 12.7854 16.8867C12.6506 17.6988 12.5334 17.8729 11.0861 19.452C9.25802 21.4565 8.46115 22.5779 7.81075 24.0797C7.31271 25.2334 6.90255 26.9285 6.89669 27.8438C6.89083 28.2369 6.92013 28.3981 7.0549 28.7139C7.23068 29.1393 7.63497 29.6356 7.99825 29.8676C8.57833 30.2479 8.32052 30.235 15.1233 30.2156L21.3576 30.1963L21.6975 30.0481C22.1662 29.8354 22.6818 29.3004 22.9221 28.7654C23.0861 28.4045 23.1096 28.282 23.1037 27.8567C23.0979 26.9285 22.6994 25.2979 22.1955 24.1055C21.551 22.5973 20.76 21.4693 18.9143 19.452C17.467 17.8664 17.3498 17.6924 17.2151 16.8867C17.1623 16.6031 17.1623 16.3969 17.2151 16.1133C17.3498 15.3012 17.467 15.1272 18.9143 13.5481C20.76 11.5307 21.551 10.4027 22.1955 8.89454C22.6994 7.71505 23.1037 6.02638 23.1037 5.12404C23.1037 4.76954 23.0686 4.58263 22.9397 4.26681C22.7287 3.74474 22.1838 3.17111 21.6975 2.95197L21.3576 2.80372L15.0881 2.79083C9.22286 2.77794 8.79513 2.78439 8.4963 2.89396ZM15.5744 19.2199C16.7756 20.393 18.4279 22.0945 18.7268 22.4684C19.9279 23.9637 20.5197 25.5299 20.5842 27.3475C20.5959 27.7729 20.5901 27.7986 20.4026 27.9791L20.2151 28.166H15.0002H9.78536L9.59786 27.9791C9.41036 27.7986 9.4045 27.7729 9.41622 27.3475C9.45138 26.3033 9.74435 25.0852 10.1721 24.1699C10.6584 23.1387 11.1916 22.4619 12.7854 20.8635C14.9182 18.7301 14.7424 18.8848 15.0002 18.8848C15.1936 18.8848 15.2815 18.9363 15.5744 19.2199Z" />
              </svg>
              <span>{{ order.timeOrdered.getHours() }}:{{ order.timeOrdered.getMinutes().toString().padStart(2, "0") }}</span>
            </span>
            <svg @click="orderInfo(order.id)" class="searchIcon" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M11 19C15.4183 19 19 15.4183 19 11C19 6.58172 15.4183 3 11 3C6.58172 3 3 6.58172 3 11C3 15.4183 6.58172 19 11 19Z" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M20.9999 20.9999L16.6499 16.6499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <button @click="finishOrder(order)" class="changeStateButton">
              <svg width="13" height="13" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path d="M20.285 2L9 13.567L3.714 8.556L0 12.272L9 21L24 5.715L20.285 2Z" />
              </svg>
              zet op afgeleverd
            </button>
          </div>
        </div>
      </section>
      <section>
        <h2>Afgeleverde bestellingen van vandaag</h2>
        <div class="orders">
          <div v-for="order in deliveredOrders" class="order orderDone">
            <span class="orderNumber">#{{ order.id.toString().padStart(3, "0") }}</span>
            <div>
              <span v-for="lane in order.reservation.lanes" class="laneNumber">{{ lane.laneNumber }}</span>
            </div>
            <span class="time">
              <svg class="sandWalkerIcon" width="30" height="33" viewBox="0 0 30 33" xmlns="http://www.w3.org/2000/svg">
                <path d="M8.4963 2.89396C7.85763 3.11954 7.32443 3.63517 7.0549 4.28614C6.92013 4.60196 6.89083 4.7631 6.89669 5.15626C6.90255 6.0715 7.30099 7.70861 7.8049 8.89454C8.44943 10.4027 9.24044 11.5307 11.0861 13.5481C12.5334 15.1272 12.6506 15.3012 12.7854 16.1133C12.8381 16.3969 12.8381 16.6031 12.7854 16.8867C12.6506 17.6988 12.5334 17.8729 11.0861 19.452C9.25802 21.4565 8.46115 22.5779 7.81075 24.0797C7.31271 25.2334 6.90255 26.9285 6.89669 27.8438C6.89083 28.2369 6.92013 28.3981 7.0549 28.7139C7.23068 29.1393 7.63497 29.6356 7.99825 29.8676C8.57833 30.2479 8.32052 30.235 15.1233 30.2156L21.3576 30.1963L21.6975 30.0481C22.1662 29.8354 22.6818 29.3004 22.9221 28.7654C23.0861 28.4045 23.1096 28.282 23.1037 27.8567C23.0979 26.9285 22.6994 25.2979 22.1955 24.1055C21.551 22.5973 20.76 21.4693 18.9143 19.452C17.467 17.8664 17.3498 17.6924 17.2151 16.8867C17.1623 16.6031 17.1623 16.3969 17.2151 16.1133C17.3498 15.3012 17.467 15.1272 18.9143 13.5481C20.76 11.5307 21.551 10.4027 22.1955 8.89454C22.6994 7.71505 23.1037 6.02638 23.1037 5.12404C23.1037 4.76954 23.0686 4.58263 22.9397 4.26681C22.7287 3.74474 22.1838 3.17111 21.6975 2.95197L21.3576 2.80372L15.0881 2.79083C9.22286 2.77794 8.79513 2.78439 8.4963 2.89396ZM15.5744 19.2199C16.7756 20.393 18.4279 22.0945 18.7268 22.4684C19.9279 23.9637 20.5197 25.5299 20.5842 27.3475C20.5959 27.7729 20.5901 27.7986 20.4026 27.9791L20.2151 28.166H15.0002H9.78536L9.59786 27.9791C9.41036 27.7986 9.4045 27.7729 9.41622 27.3475C9.45138 26.3033 9.74435 25.0852 10.1721 24.1699C10.6584 23.1387 11.1916 22.4619 12.7854 20.8635C14.9182 18.7301 14.7424 18.8848 15.0002 18.8848C15.1936 18.8848 15.2815 18.9363 15.5744 19.2199Z" />
              </svg>
              <span>{{ order.timeOrdered.getHours() }}:{{ order.timeOrdered.getMinutes().toString().padStart(2, "0") }}</span>
            </span>
            <span class="time">
              <svg class="timeDeliveredIcon" width="13" height="13" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path d="M20.285 2L9 13.567L3.714 8.556L0 12.272L9 21L24 5.715L20.285 2Z" />
              </svg>
              <span>{{ order.timeDelivered.getHours() }}:{{ order.timeDelivered.getMinutes().toString().padStart(2, "0") }}</span>
            </span>
            <svg @click="orderInfo(order.id)" class="searchIcon" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M11 19C15.4183 19 19 15.4183 19 11C19 6.58172 15.4183 3 11 3C6.58172 3 3 6.58172 3 11C3 15.4183 6.58172 19 11 19Z" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M20.9999 20.9999L16.6499 16.6499" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <button @click="finishOrder(order)" v-if="!order.isDelivered" class="changeStateButton">
              <svg width="13" height="13" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path d="M20.285 2L9 13.567L3.714 8.556L0 12.272L9 21L24 5.715L20.285 2Z" />
              </svg>
              zet op afgeleverd
            </button>
          </div>
        </div>
      </section>
    </div>
  </main>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { isOrderOnTime } from '../store/functions';
import type { Order } from '!/ApiClient';
import { OrderClient } from '!/ApiClient';
import loader from '@/components/loader.vue';

export default defineComponent({
  components: {
    loader
  },
  data() {
    return {
      loading: false,
      orders: [] as Order[],
      deliveredOrders: [] as Order[]
    }
  },
  methods: {
    orderInfo(id: number): void {
      this.$router.push(`/managementOrder/${id}`)
    },
    getUnreadyOrders(): Order[] {
      return this.orders.filter((order) => !order.isReady);
    },
    getReadyOrders(): Order[] {
      return this.orders.filter((order) => order.isReady);
    },
    finishOrder(order: Order): void {
      this.loading = true;
      // @ts-ignore
      new OrderClient(null, this.http).setOrderDelivered(order.id, true)
          .then(() => {
            this.$toast.success("Bestelling afgerond!");
            this.fetchOrders();
            this.fetchOldOrders();
          })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
          .finally(() => {
            this.loading = false;
          })
    },
    fetchOrders() {
      this.loading = true;
      // @ts-ignore
      new OrderClient(null, this.http).getAll()
          .then((orders: Order[]) => {
            this.orders = orders;
          })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
          .finally(() => {
            this.loading = false;
          });
    },
    fetchOldOrders() {
      this.loading = true;
      // @ts-ignore
      new OrderClient(null, this.http).getDeliveredOrders()
          .then((orders: Order[]) => {
            this.deliveredOrders = orders;
          })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
          .finally(() => {
            this.loading = false;
          });;
    },
    isOrderOnTime
  },
  mounted() {
    this.fetchOrders();
    this.fetchOldOrders();
  }
})
</script>



<style scoped lang="scss">
@import '@/assets/css/colors.scss';

main {
  width: 90vw;
  margin-left: 5vw;
  
  > h1 {
    text-align: center;
  }
}

.timeDeliveredIcon {
  margin-right: 5px;
  scale: 1.25;
}

.content {
  display: flex;
  justify-content: space-between;

  > section {
    margin-top: 2rem;
    width: 45%;

    > h2 {
      text-align: center;
      height: 59px;
      display: flex;
      align-items: end;
      justify-content: start;
    }
  }
}

.order {
  padding: 10px 15px;
  border-radius: 10px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
  //min-width: 300px;
  width: 90%;
  &:not(:last-child) {
    margin-bottom: 10px;
  }

  > .changeStateButton {
    color: $creme;
    fill: $creme;
    stroke: $creme;
    background-color: $green-3;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 5px;
    padding: 10px 15px;
    border: none;
    border-radius: 10px;
    height: 30px;
    cursor: pointer;

    > svg {
      transition: rotate 0.2s;
    }

    &:hover {
      background-image: linear-gradient(rgb(0 0 0/20%) 0 0);
    
      > svg {
        rotate: 15deg;
      }
    }

  }

  > .searchIcon {
    background-color: #D9D9D9;
    padding: 5px;
    border-radius: 50%;
    width: 20px;
    height: 20px;
    transition: scale 0.1s;
    cursor: pointer;

    &:hover {
      scale: 1.1;
      background-image: linear-gradient(rgb(0 0 0/20%) 0 0);
    }
  }

  > .time, > .onTime {
    display: flex;
    justify-content: center;
    align-items: center;

    > svg {
      height: 25px;
    }
  }

   .laneNumber {
    text-align: center;
    display:inline-block;
    width: 30px;
    height: 30px;
    font-weight: bold;
    color: white;
    font-size: 1.5rem;
    border-radius: 50%;
    background-color: $blue;
  }
}

.orderDone {
  background-color: $green-2;
  color: $green-3;
  stroke: $green-3;
  fill: $green-3;
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

.open {
  color: $gray-2;
  background-color: $gray;
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

.lateOpen {
  color: $red-5;
  background-color: $red-4;
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