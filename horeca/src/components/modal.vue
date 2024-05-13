<template>
  <portal>
    <div class="modal" :class="{ open, closing, fullscreen, large }">
      <div class="modal-bg" @click="toggle"></div>
      <div class="modal-inner" :style="width ? `max-width: ${width}px` : null">
        <div class="modal-header">
          <div class="modal-header-left">
            <div class="title">
              <h2>{{ title }}</h2>
            </div>
            <div class="alt-1"></div>
            <div class="alt-2"></div>
          </div>
          <div class="modal-header-right" v-if="!preventClose">
            <div @click="toggle" class="closeBTN">
              <img src="@/assets/img/icon/x-circle.svg" alt="Close.png">
            </div>
          </div>
        </div>

        <div class="modal-body">
          <div class="modal-body-inner" ref="bodyInner">
            <slot></slot>
          </div>
        </div>

        <div v-if="$slots.footer" class="modal-footer">
          <slot name="footer"></slot>
        </div>
      </div>
    </div>
  </portal>
</template>

<script lang="ts">
//HOW TO USE
// <div v-if="modal" class="modalContainer">
//   <modal :prevent-close="false" :large="true" title="testTitle" @close="closeModal">
//   <div style="height: 500px; width: 1000px"></div> //content form
// </modal>
// </div>

// data() {
//   return {
//     modal : false
//   }
// },
// methods:{
//   openModal(){
//     this.modal = true
//   },
//
//   closeModal(){
//     this.modal = false
//   }
// }
import {defineComponent} from "vue";

export default defineComponent({
  name: 'modal',
  props: {
    title: String,
    fullscreen: Boolean,
    width: String,
    preventClose: Boolean,
    large: Boolean,
  },

  data() {
    return {
      open: false,
      closing: false
    }
  },
  mounted() {
    this.$nextTick(() => {
      this.toggle()
    })
  },
  methods: {
    toggle() {
      if (this.preventClose) {
        return false
      }

      this.open = !this.open

      if (!this.open) {
        this.closing = true
        setTimeout(() => {
          this.$emit('close')
          // location.reload();
        }, 400)
      }
    }
  }
})
</script>

<style lang="scss" scoped>
@import "@/assets/css/colors.scss";
.closeBTN {
  width: 50px;
  height: 50px;
  background: #ff3e3e;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 50%;
  //border-radius: .7rem;
  cursor: pointer;
  transition: all .22s;

  img {
    width: 90%;
    filter: invert(1);
    transition: all .22s;
  }

  &:hover {
    transform: scale(1.1);
  }
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 110;

  &.hidden {
    pointer-events: none;
  }
}

.modal-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: black;
  opacity: 0;
  cursor: pointer;
  animation: modal-bg-open .66s forwards;
}

@keyframes modal-bg-open {
  from {
    opacity: 0;
  }

  to {
    opacity: .8;
  }
}

@keyframes modal-bg-closed {
  to {
    opacity: 0;
  }

  from {
    opacity: .8;
  }
}

.modal-inner {
  display: flex;
  flex-direction: column;
  background-color: $creme;
  position: relative;
  z-index: 2;
  width: 100%;
  max-width: 800px;
  transition: opacity .25s .5s;
  overflow: hidden;
  max-height: calc(100vh - 120px);
  will-change: transform, clip-path;
  animation: modal-body-open 1.5s forwards cubic-bezier(0.26, 0.88, 0.15, 0.99);
}

@keyframes modal-body-open {
  0% {
    transform: scale(.8);
    clip-path: circle(0 at 50% 50%);
  }

  66% {
    transform: none;
  }

  100% {
    transform: none;
    clip-path: circle(75vmax at 50% 50%);
  }
}

@keyframes modal-body-closed {
  0% {
    transform: none;
  }

  15% {
    clip-path: circle(75vmax at 50% 50%);
  }

  100% {
    transform: scale(.9);
    clip-path: circle(0 at 50% 50%);
  }
}

.modal.closing {
  .modal-bg {
    opacity: .8;
    animation: modal-bg-closed .2s .1s forwards;
  }

  .modal-inner {
    animation: modal-body-closed .4s forwards cubic-bezier(0.6, 0, 0.4, 1);
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: $dark;
  border-bottom: 2px solid var(--mono2);
  width: 100%;
  height: 10%;
  min-height: 80px;

  .modal-header-left {
    width: 70%;
    height: 100%;
    //padding: 16px;
    color: $creme;
    background: $orange;
    display: flex;

    .title{
      padding: 0 1rem 0 1rem;
      display: flex;
      align-items: center;
      justify-content: center;
      background: $orange;
      width: 90%;
    }

    .alt-1, .alt-2{
      height: 100%;
      width: 5%;
    }
    .alt-1{
      background: $orange-2;
    }
    .alt-2{
      background: $red;
    }
  }

  .modal-header-right {
    padding: 16px;
  }
}

.modal-body {
  position: relative;
  overflow: hidden;
  overscroll-behavior: contain;
  overflow-y: scroll;
  height: 100%;
  -ms-overflow-style: none;
  /* Internet Explorer 10+ */
  scrollbar-width: none;
  /* Firefox */

  &::-webkit-scrollbar {
    display: none;
  }

  .modal-body-inner {
    //padding: 16px;
    padding: 0;
    height: 100%;
  }
}

.modal-footer {
  padding: 16px;
  background: #FFF;
  border-top: 2px solid var(--mono2);
}

//@media (min-width: 768px) {
.modal {
  // display: flex;
  // justify-content: center;
  // align-items: center;
  display: grid;
  place-content: center;
  //grid-template-columns: min(800px, calc(100vw - 16px));

  &.large {
    grid-template-columns: 90%;
    grid-template-rows: 90%;

    .modal-inner {
      max-width: 100%;

      .modal-body {
        max-height: 100%;

        .modal-body-inner {
          //padding: 32px;
          //margin-bottom: 80px;
        }
      }
    }
  }

  &.fullscreen {
    grid-template-columns: 100%;
    place-content: unset;

    .modal-inner {
      // border: 2px solid var(--mono2);
      border-radius: 0px;
      max-width: 100%;

      .modal-body {
        max-height: 100%;

        .modal-body-inner {
          //padding: 32px;
          //margin-bottom: 80px;
        }
      }
    }
  }
}

.modal-inner {
  // border: 2px solid var(--mono2);
  border-radius: 24px 24px 32px 32px;
}
//}
</style>