<template style="margin-top:20px;" >
    
    
    <div class="row">

        
          <div  class="col-4">
              <label for="name" class="titleItem">Name</label>
              <div class="viewItem" v-if="viewMode">
                {{ idea.name }}
              </div>
              <md-field v-else :class="getValidationClass('name')">
              <md-input type="name" name="name" id="name" autocomplete="name" v-model="idea.name" :disabled="sending" />
              <span class="md-error" v-if="!$v.idea.name.required">The name is required</span>
              <span class="md-error" v-else-if="!$v.idea.name.name">Invalid name</span>
            </md-field>

          </div>

       
          <div class="col-8">
          
              <div class="md-layout md-gutter">
                  
                  <div class="md-layout-item md-size-20">
                      <label for="impact" class="titleItem">Impact</label>

                      <div class="viewItem" v-if="viewMode">
                        {{ idea.impact }}
                      </div>
                      <md-field v-else>
                      <!-- <label for="impact">Impact</label> -->
                      <md-select v-model="idea.impact" name="impact" id="impact">

                          <md-option v-for="point in points" :key="point" :value="point">{{ point }}</md-option>

                          
                      </md-select>
                      </md-field>
                  </div>

                  <div class="md-layout-item md-size-20">
                      <label for="ease" class="titleItem">Ease</label>
                      <div class="viewItem" v-if="viewMode">
                        {{ idea.ease }}
                      </div>
                      <md-field v-else>
                      <md-select  v-model="idea.ease" name="ease" id="ease">

                          <md-option v-for="point in points" :key="point" :value="point">{{ point }}</md-option>

                          
                      </md-select>
                      </md-field>
                  </div>

                  <div class="md-layout-item md-size-20">
                      <label for="confidence" class="titleItem">Confidence</label>
                       <div class="viewItem" v-if="viewMode">
                        {{ idea.confidence }}
                      </div>
                      <md-field v-else>
                      <md-select v-model="idea.confidence"  name="confidence" id="confidence">

                          <md-option v-for="point in points" :key="point" :value="point">{{ point }}</md-option>

                          
                      </md-select>
                      </md-field>
                  </div>

                  <div class="md-layout-item md-size-20">
                    <label class="titleItem">Avg.</label>
                      <div v-if="viewMode==false" class="viewItem" style="margin-top: 30px;">
                        <!-- {{idea.ease}} -->
                        <span>{{avg}}</span>
                      </div>
                      <div v-else class="viewItem" >
                        <span>{{avg}}</span>
                      </div>
                      
                      
                  </div>

                  <div class="md-layout-item md-size-20">
                      <div  >
                        <!-- {{idea.ease}} -->
                        <div v-if="viewMode==false" class="action-edit" style="margin-top: 58px;">
                          <a href="#"  @click.prevent="validateData" class="mr-2">
                            <img src="../assets/Confirm_V.png"  alt />
                          </a>
                          <a href="#"  @click.prevent="cancel">
                            <img src="../assets/Cancel_X.png"  alt />
                          </a>
                        </div>

                        <div v-else  class="action-edit">
                          <a href="#"  @click.prevent="setEditMode" class="mr-2">
                            <img src="../assets/pen.png"  alt />
                          </a>
                          <a href="#"  @click.prevent="deleteIdea">
                            <img src="../assets/bin.png"  alt />
                          </a>
                        </div>
                       
                      </div>
                      
                      
                  </div>

              </div>

          </div>
          <div class="col-12">
              <span v-if="apiError!=null">{{apiError}}</span>
          </div>
          <!-- <form style="margin:0 auto;" novalidate  @submit.prevent="validateData">

        </form> -->
      
    </div>

    

</template>

<script>
import Vue from 'vue'

  import { validationMixin } from 'vuelidate'
  import {
    required,
    email,
    minLength,
    maxLength
  } from 'vuelidate/lib/validators'

  import axios from 'axios';




export default Vue.extend({
props: {
    jwt:{
      type: String,
      required: true,
    },
    refresh_token:{
      type: String,
      required: true,
    },
    id:null,
    user: {
      type: Object,
      required: true
    },
    ideaRec:{
       type: Object,
    }
  },
data() {
    const pointsArray=[...new Array(10)].map((_,i) => i+1);
      const idea={
              name:null,
              impact:10,
              ease:10,
              confidence:10,
      };
      if(this.ideaRec!=null){
        idea.name=this.ideaRec.content;
        idea.ease=this.ideaRec.ease;
        idea.impact=this.ideaRec.impact;
        idea.confidence=this.ideaRec.confidence;
      }
      const isViewMode=this.ideaRec!=null;
    return {
        points:pointsArray,
        sending: false,
        apiError:null,
        ideaId:this.id,
        viewMode:isViewMode,
        idea:idea
    }
},
 validations: {
      idea: {
        name: {
          required,
          minLength: minLength(10)
        },
        impact: {
          required
        },
        ease: {
          required
        },
        confidence: {
          required
        }
      }
    },
computed:{
    avg(){
        const calc=((this.idea.impact + this.idea.ease + this.idea.confidence)/3);
        return calc.toFixed(2);
    },
    isEdit(){
      return this.ideaRec!=null;
    }
},

methods:{

    getValidationClass (fieldName) {
        const field = this.$v.idea[fieldName]

        if (field) {
          return {
            'md-invalid': field.$invalid && field.$dirty
          }
        }
      },
    insert(jsonData,config){

       axios.post("https://localhost:5001/ideas",jsonData,config).then(res=>{
          console.log(res);
          this.sending = false;
          this.clearForm();
          this.$emit("idea-added",res.data);
          // this.$emit('user-logged',res.data);
          this.apiError="";
        }).catch(reason=>{
            if(reason.response.data!=null && reason.response.data.message!=null){
                this.apiError=reason.response.data.message;
            }
            else{
                this.apiError="An Error Ocurred..";
            }
            this.sending = false;
        });

    },
    update(id,jsonData,config){

        axios.put("https://localhost:5001/ideas/"+id,jsonData,config).then(res=>{
          console.log(res);
          this.sending = false;
          this.clearForm();
          this.viewMode=true;
          this.$emit("idea-updated",res.data);
          // this.$emit('user-logged',res.data);
          this.apiError="";
        }).catch(reason=>{
            if(reason.response.data!=null && reason.response.data.message!=null){
                this.apiError=reason.response.data.message;
            }
            else{
                this.apiError="An Error Ocurred..";
            }
            this.sending = false;
        });

    },
    save(){
        
        this.sending = true;
        const jsonData={
        "content": this.idea.name,
        "confidence": this.idea.confidence,  
        "ease": this.idea.ease, 
        "impact": this.idea.impact,
        "id":0
        };
        const config={
            headers:{"content-type": "application/json",
            "Access-Control-Allow-Origin": "*",
            "X-Access-Token":this.jwt
            }
        };
        if(this.isEdit){
          jsonData.id=this.ideaRec.id;

          this.update(jsonData.id,jsonData,config);
        }
        else{
          this.insert(jsonData,config);
        }
        
       

        
    },
    validateData () {
        this.$v.$touch();
        
        if (!this.$v.$invalid) {
          this.save();
        }
    },
    cancel(){
      //in case we are editiing the idea
      
      if(this.ideaRec!=null){
        this.viewMode=true;
      }
      else{
        this.$emit("cancel");
      }
    },
     clearForm () {
        this.$v.$reset();
        this.idea.name = null;
        this.idea.confidence = 10;
        this.idea.ease = 10;
        this.idea.impact = 10;
      },

    setEditMode(){
      this.viewMode=false;
      //
    },
    deleteIdea(){
       const config={
            headers:{"content-type": "application/json",
            "Access-Control-Allow-Origin": "*",
            "X-Access-Token":this.jwt
            }
        };

        axios({
      method: 'DELETE',
      url: 'https://localhost:5001/ideas/'+this.ideaRec.id,
      config : config
      }).then(res=>{
       this.$emit("idea-updated",null);
      });
      //
    }


}

    
})
</script>

<style lang="scss" scoped>

.editItem{
  margin-top:25px
}

.viewItem{
  margin-top:10px;
  margin-top: 5px;
    margin-bottom: 25px;
    border-bottom: 1px solid gray;
}
.titleItem{
  color:black;
  font-weight: bold;
}
.action-edit{
    margin-top: 31px;
    float:unset !important;
}
</style>