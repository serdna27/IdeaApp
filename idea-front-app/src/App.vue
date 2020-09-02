<template>
  <div id="app">
    <!-- <img alt="Vue logo" src="./assets/logo.png"> -->
    <!-- <HelloWorld msg="Welcome to Your Vue.js + TypeScript App"/> -->
    <!-- <Home /> -->
    <Container />
  </div>
</template>

<script >
import Vue from 'vue';


import VueMaterial from 'vue-material'
import 'vue-material/dist/vue-material.min.css'

Vue.use(VueMaterial);

import HelloWorld from './components/HelloWorld.vue';
import Container from './components/Container.vue';



import 'vue-material/dist/vue-material.min.css'
import 'vue-material/dist/theme/default.css'





import Vuelidate from 'vuelidate'
Vue.use(Vuelidate)

export default Vue.extend({
  name: 'App',
  components: {
    Container
  }
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
