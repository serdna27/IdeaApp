<template>
  <div >
    <form style="margin:0 auto;" novalidate class="md-layout" @submit.prevent="validateUser">
      <md-card class="md-layout-item md-size-50 md-small-size-100">
        <md-card-header>
          <div class="md-title">Login</div>
        </md-card-header>

        <md-card-content>
   

             <md-field :class="getValidationClass('email')">
            <label for="email">Email</label>
            <md-input type="email" name="email" id="email" autocomplete="email" v-model="form.email" :disabled="sending" />
            <span class="md-error" v-if="!$v.form.email.required">The email is required</span>
            <span class="md-error" v-else-if="!$v.form.email.email">Invalid email</span>
          </md-field>

            <div class="md-layout-item md-small-size-100">
              <md-field :class="getValidationClass('password')">
                <label for="last-name">Password</label>
                <md-input name="last-name" type="password" id="last-name" autocomplete="family-name" v-model="form.password" :disabled="sending" />
                <span class="md-error" v-if="!$v.form.password.required">The password is required</span>
                <span class="md-error" v-else-if="!$v.form.password.minlength">Invalid password</span>
              </md-field>
            </div>

           
          
        </md-card-content>

        <md-progress-bar md-mode="indeterminate" v-if="sending" />
        
        <span style="color:red;" v-if="apiError!=null">{{ apiError }}</span>

     <md-card-actions style="justify-content: flex-start;">
          <md-button type="submit" class="md-primary main-button" :disabled="sending">Log In</md-button>

            <span style="color:gray;margin-left:8px;font-size:13px">
                Don't have an account? 
                <a href="#" class="main-text-color" @click="signup">
                Create an accout
              </a>
            </span>
            
        </md-card-actions>
        
      </md-card>

      <md-snackbar :md-active.sync="userSaved">The user {{ lastUser }} was saved with success!</md-snackbar>
    </form>
  </div>
</template>

<script>
  import { validationMixin } from 'vuelidate'
  import {
    required,
    email,
    minLength,
    maxLength
  } from 'vuelidate/lib/validators'

  import axios from 'axios';


  export default {
    name: 'FormValidation',
    mixins: [validationMixin],
    data: () => ({
      form: {
        password: null,
        email: null,
      },
      userSaved: false,
      sending: false,
      lastUser: null,
      apiError:null,
    }),
    validations: {
      form: {
        password: {
          required,
        },
        email: {
          required,
          email
        }
      }
    },
    methods: {
      getValidationClass (fieldName) {
        const field = this.$v.form[fieldName]

        if (field) {
          return {
            'md-invalid': field.$invalid && field.$dirty
          }
        }
      },
      clearForm () {
        this.$v.$reset()
        this.form.firstName = null
        this.form.password = null
        this.form.age = null
        this.form.gender = null
        this.form.email = null
      },
      login () {
        this.sending = true;

        const jsonData={
        "email": this.form.email,
        "password": this.form.password  
        };
        const config={
            headers:{"content-type": "application/json","Access-Control-Allow-Origin": "*"}
        };
        // axios.get("https://localhost:5001/ideas/1").then();

        axios.post("https://localhost:5001/access-tokens",jsonData,config).then(res=>{
          console.log(res);
          this.userSaved = true;
          this.sending = false;
          this.clearForm()
          this.$emit('user-logged',res.data);
          this.apiError="";
        }).catch(reason=>{
          debugger;
            if(reason.response.data!=null && reason.response.data.message!=null){
                this.apiError=reason.response.data.message;
            }
            else{
                this.apiError="An Error Ocurred..";
            }
            this.sending = false;
        });

        // Instead of this timeout, here you can call your API
        // window.setTimeout(() => {
        //   this.lastUser = `${this.form.firstName} ${this.form.password}`
        //   this.userSaved = true
        //   this.sending = false
        //   this.clearForm()
        // }, 1500);
      },
      validateUser () {
        this.$v.$touch()
        
        if (!this.$v.$invalid) {
          this.login()
        }
      },
      signup(){
          this.$emit('show-login-form',false);
      }
    }
  }
</script>

<style lang="scss" scoped>
  .md-progress-bar {
    position: absolute;
    top: 0;
    right: 0;
    left: 0;
  }
   .main-text-color{
    color: rgba(0,168,67,1) !important;  
  }
   .main-button{
    background-color: rgba(0,168,67,1);
    color:white !important;
}
</style>