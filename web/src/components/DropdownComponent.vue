<template>
  <div class="dropdown-container">
    <div class="dropdown-header">
      <p style="font-weight: bold">{{title}}</p>
      <div class="close-container">
        <img class="close-btn" @click="$emit('close')" src="@/assets/img/icon/x-circle.svg" alt="">
      </div>
    </div>
    <slot>

    </slot>
  </div>


</template>

<script lang="ts">
import {defineComponent} from "vue";

export default defineComponent({
  props: {
    title: { type: String, required: true },
    width: { type: String, required: true },
    height: { type: String, required: true },
    posX: { type: String, required: true },
    posY: { type: String, required: true },
  }
})
</script>

<style scoped lang="scss">
@import "@/assets/css/colors.scss";
.dropdown-container{
  position: absolute;
  top: v-bind(posY);
  left: v-bind(posX);
  min-width: 250px;
  width: v-bind(width);
  height: v-bind(height);
  background: $creme;
  border-radius: 10px;
  box-shadow: 0 0 10px black;
}

.dropdown-header{
  padding: 0 1rem 0 1rem;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  //border-bottom: 2px solid $dark;
  height: 20%;
  background: $orange-5;

  .close-container{
    background: red;
    height: 28px;
    width: 28px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;

    img{
      filter: invert(1);

    }
  }
}

</style>