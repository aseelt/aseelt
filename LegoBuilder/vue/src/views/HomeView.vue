<template>
  <div class="home">
    <h1>Home</h1>
    <p>You must be authenticated to see this</p>
    <button v-on:click="getRandomSet()">Get Me a Random Set</button>
    <full-set-info v-bind:fullSetInfo="fullSetInfo"></full-set-info>
    <!-- <p v-for="set in this.sets_parts" v-bind:key="set.set_num">{{ set.set_num }}</p> -->
  </div>
</template>

<script>
import GetRandomSetService from '../services/GetRandomSetService'
import FullSetInfo from '../components/FullSetInfo.vue'

export default {
  components: {
    FullSetInfo
  },
  data() {
    return {
      fullSetInfo: {}
    }
  },
  methods: {
    getRandomSet(){
      GetRandomSetService.getSet()
      .then( response => {
        this.fullSetInfo = response.data;
      })
      .catch( function (error) {
        this.handleErrorResponse(error);
      })

    },
    handleErrorResponse(error) {
      console.log('Oops, something went wrong')
  },
  },
  
  created(){
    this.getRandomSet();
  }
};
</script>
