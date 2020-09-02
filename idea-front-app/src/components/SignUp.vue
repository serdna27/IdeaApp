<template>
  <div >
    <form style="margin:0 auto;" novalidate class="md-layout" @submit.prevent="validateUser">
      <md-card class="md-layout-item md-size-50 md-small-size-100">
        <md-card-header>
          <div class="md-title">Sign Up</div>
        </md-card-header>

        <md-card-content>
          <div class="md-layout md-gutter">
            <div class="md-layout-item md-small-size-100">
              <md-field :class="getValidationClass('firstName')">
                <label for="first-name">Name</label>
                <md-input name="first-name" id="first-name" autocomplete="given-name" v-model="form.firstName" :disabled="sending" />
                <span class="md-error" v-if="!$v.form.firstName.required">The first name is required</span>
                <span class="md-error" v-else-if="!$v.form.firstName.minlength">Invalid first name</span>
              </md-field>
            </div>
            </div>

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

        <md-card-actions>
          <md-button type="submit" class="md-primary" :disabled="sending">Create user</md-button>
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
        firstName: null,
        password: null,
        gender: null,
        age: null,
        email: null,
      },
      userSaved: false,
      sending: false,
      lastUser: null,
      apiError:null,
    }),
    validations: {
      form: {
        firstName: {
          required,
          minLength: minLength(3)
        },
        password: {
          required,
          minLength: minLength(8)
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
      saveUser () {
        this.sending = true;

        const jsonData={
        "email": this.form.email,
        "name": this.form.firstName,
        "password": this.form.password  
        };
        const config={
            headers:{"content-type": "application/json","Access-Control-Allow-Origin": "*"}
        };
        // axios.get("https://localhost:5001/ideas/1").then();

        axios.post("https://localhost:5001/users",jsonData,config).then(res=>{
          console.log(res);
          this.userSaved = true;
          this.sending = false;
          this.clearForm()
        }).catch(reason=>{
            if(reason.response.data!=null){
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
          this.saveUser()
        }
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
</style>