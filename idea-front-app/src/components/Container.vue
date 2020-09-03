<template>
  <div class="page-container">
    <md-app md-waterfall md-mode="fixed">
      <!-- <md-app-toolbar class="md-primary">
        <span class="md-title">My Title</span>
      </md-app-toolbar> -->

      <md-app-drawer md-permanent="full" class="left-menu">
        <md-toolbar class="md-transparent" md-elevation="0">
          <div style="margin:0 auto; padding-top:10px;">
          <img src="../assets/IdeaPool_icon.png" alt="">
            <h4>The Idea Pool</h4>
            
          </div>
        </md-toolbar>
        <hr style="background-color:gray;">
        <md-list>
          <md-list-item class="left-menu">

            <div v-if="isUserSet"  style="margin:0 auto; padding-top:10px;" >
              <img src="https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d50" width="60px;">
              
              <a href="#" class="md-list-item-text" style="color:white" @click="logout">
                Log Out
              </a>
                <!-- <span class="md-list-item-text" style="margin-left:10px;">User</span> -->
                
            </div>

            <!-- <md-icon>move_to_inbox</md-icon> -->
          
          </md-list-item>

        
        </md-list>
      </md-app-drawer>

      <md-app-content>

        <Signin v-if="showLoginForm" v-on:user-logged="populateTokens" v-on:show-login-form="showLogin"
          />

        <SignUp v-if="showSignUpForm" v-on:user-logged="populateTokens" v-on:show-login-form="showLogin"
          />

      </md-app-content>
    </md-app>
  </div>
</template>

<script >

import Vue from 'vue'
import SignUp from "./SignUp.vue";
import Signin from "./Signin.vue";

import axios from 'axios';

// interface ResultToken{
//   jwt: string;
//   refresh_token: string;
// }

export default Vue.extend({
  name: 'Waterfall',
   components:{
    SignUp,Signin
  },
  data(){

    return {

      jwt:"",
      refresh_token:"",
      showLoginForm:false,
      showSignUpForm:true,
      user:{
        email: null,
        name: null,
        avatar_url: null
      }
    }
  },
  computed:{

    validToken(){
      return this.jwt!="" && this.refresh_token!="";
    },
    isUserSet(){
      return this.user.email!=null;
    }
  },
  methods:{
    
    populateTokens(result){
      this.jwt=result.jwt;
      this.refresh_token=result.refresh_token;
      console.log("populate tokens and fill user");
        const config={
            headers:{
            "content-type": "application/json",
            "Access-Control-Allow-Origin": "*",
            "X-Access-Token":this.jwt
            }
        };

      axios.get("https://localhost:5001/me",config).then(res=>{
        this.user=res.data;
      })
      

    },

    logout(){

        console.log("populate tokens and fill user");
        const config={
            headers:{
            "content-type": "application/json"
            // "Access-Control-Allow-Origin": "*",
            // "refresh_token":this.refresh_token
            }
        };

        axios({
      method: 'DELETE',
      url: 'https://localhost:5001/access-tokens',
      data: {refresh_token:this.refresh_token},
      config : config
      }).then(res=>{
           this.user.email=null;
        this.user.name=null;
        this.user.avatar_url=null;
        this.jwt=null;
        this.refresh_token=null;
      });

   

    },
    showLogin(showLogin){
      this.showLoginForm = showLogin;
      this.showSignUpForm = !showLogin;
    }


  }
  
})
</script>

<style lang="scss" scoped>

@import "~vue-material/dist/theme/engine"; // Import the theme engine


  .md-app {
    max-height: 400px;
    border: 1px solid rgba(#000, .12);
  }

   // Demo purposes only
  .md-drawer {
    width: 230px;
    max-width: calc(100vw - 125px);
    min-height: 400px;
  }
 .left-menu{
    background-color: rgba(0,168,67,1);
    color:white;
}

.md-drawer {
    background: rgba(0,168,67,1) !important; // makes the drawer partially blue
    color:white !important;;
}

.md-list.md-theme-default{
      background-color: rgba(0,168,67,1) !important;
      // var(--md-theme-default-background, rgb(77, 143, 3));
}

</style>

