<template>
  <hero></hero>
  <welkom-text></welkom-text>
</template>

<script>
import {defineComponent} from "vue";
import hero from "@/components/Hero.vue";
import welkomText from "@/components/WelkomText.vue";
export default defineComponent({
  components:{hero, welkomText}
})
</script>