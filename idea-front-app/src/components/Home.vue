<template >
    <div class="row">
        <div class="col-2 left-menu" >
            menu
        </div>

         <div class="col-10 main" >
             <!-- <h2>mmmg</h2> -->
            <sign-up/>
            
        </div>

    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import SignUp from '../components/SignUp.vue'

export default Vue.extend({
    components:{
        SignUp
    }
    
})
</script>

<style lang="scss" scoped>


@import 'node_modules/bootstrap/scss/bootstrap';
@import 'node_modules/bootstrap-vue/src/index.scss';

.left-menu{
    background-color: rgba(0,168,67,1);
    color:white;
}

</style>