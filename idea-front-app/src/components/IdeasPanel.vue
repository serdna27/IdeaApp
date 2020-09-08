<template>
  <div class="container">
    <div class="row">
       
        <div class="col-10">
             <h3>My Ideas</h3>
        </div>

        
        <div class="col-2">
        <a href="#" class="float-right" @click.prevent="addIdea">

            <img src="../assets/btn_addanidea.png" width="40"  alt />
            </a>
        </div>
      
    </div>
    <div class="col-12">
        
      <Idea v-if="showIdeaForm" v-on:cancel="cancelForm" :user="user"
        v-on:idea-added="loadData"
       :jwt="jwt" :refresh_token="refresh_token" />
      <br/>

        <md-progress-bar md-mode="indeterminate" v-if="loading" />

        <Idea v-for="idea in ideas" :key="idea.id" :id="idea.id"
        :ideaRec="idea"  v-on:idea-updated="loadData"
          v-on:cancel="cancelForm" :user="user" :jwt="jwt" :refresh_token="refresh_token" />

        <md-button class="md-primary main-button" @click.prevent="loadMore"
         :disabled="ideas.length<10">Load More</md-button>

     
          
    </div>
  </div>
</template>

<script>



import Vue, { PropType } from "vue";

import axios from 'axios';

// import Idea from './Idea.vue';

import Idea from './Idea.vue';

//  idea:{
//             name:null,
//             impact:10,
//             ease:10,
//             confidence:10,
//         }

export default Vue.extend({
    components:{
        Idea
    },

  data(){
      return {
          showIdeaForm:false,
          loading:false,
          ideas:[],
          currentPage:1
      }
  },
  props: {
    jwt: String,
    refresh_token: String,
    user: {
      type: Object,
      required: true,
    },
  },
  created(){
    this.loadData();
  },
  methods:{

      loadData(){

          if(this.currentPage>1){
            this.loadMore();
            return;
          }
          const config={
            headers:{"content-type": "application/json",
            "Access-Control-Allow-Origin": "*",
            "X-Access-Token":this.jwt
            }
        };
        this.ideas=[];
        
        axios.get("https://localhost:5001/ideas",config).then(res=>{
          this.ideas=res.data;
        });

      },

      addIdea(){
          this.showIdeaForm=true;
      },
      
      cancelForm(){
          this.showIdeaForm=false;
      },

      loadMore(){
          const config={
            headers:{"content-type": "application/json",
            "Access-Control-Allow-Origin": "*",
            "X-Access-Token":this.jwt
            }
        };
      
        this.currentPage=this.currentPage + 1;
        axios.get(`https://localhost:5001/ideas?page=${this.currentPage}`,config).then(res=>{

          if(res.data.length==0){
            this.currentPage=this.currentPage - 1;
          }
 

          res.data.forEach(el => {
            this.ideas.push(el);
          });

        });
      }
  }
});
</script>

<style lang="scss" scoped>
</style>