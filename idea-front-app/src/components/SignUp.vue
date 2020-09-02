<template class="container">
    <div class="row">
         <div class="col-md-4 mx-auto">
             <h1>Sign Up</h1>
                <!-- @submit="onSubmit" @reset="onReset" v-if="show" -->

                   <b-form >
                    <b-form-group
                        id="input-group-1"
                        label="Email address:"
                        label-for="input-1"
                        description="We'll never share your email with anyone else.">
                        <b-form-input
                        id="input-1"
                        v-model="form.email"
                        type="email"
                        required
                        placeholder="Enter email"
                        ></b-form-input>
                    </b-form-group>
                 </b-form>
       

         </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'

import {BForm,BFormInput,BFormGroup} from 'bootstrap-vue'

export default Vue.extend({
    data(){
        return {
            form: {
            email: '',
            name: '',
          
            }
        };
    },
    components:{
        BForm,BFormInput,BFormGroup
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