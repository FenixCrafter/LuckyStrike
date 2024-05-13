<script lang="ts">
import {defineComponent} from 'vue'
import { UserClient} from '!/ApiClient';

export default defineComponent({
  name: "ConfirmView",
  mounted(){
    //@ts-ignore
    new UserClient(null, this.http).confirmEmail(this.$route.params.token, this.$route.params.id).then(() => {
      this.$router.push("/");
      this.$toast.success("Email bevestigd");
    }).catch((msg: any) => {
      this.$toast.error("Email niet bevestigd, probeer het opnieuw");
    })
  }
})
</script>

<template>

</template>

<style scoped lang="scss">

</style>