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

        <!-- <md-list>
          <md-list-item>
            <md-icon>move_to_inbox</md-icon>
            <span class="md-list-item-text">Inbox</span>
          </md-list-item>

          <md-list-item>
            <md-icon>send</md-icon>
            <span class="md-list-item-text">Sent Mail</span>
          </md-list-item>

          <md-list-item>
            <md-icon>delete</md-icon>
            <span class="md-list-item-text">Trash</span>
          </md-list-item>

          <md-list-item>
            <md-icon>error</md-icon>
            <span class="md-list-item-text">Spam</span>
          </md-list-item>
        </md-list> -->
      </md-app-drawer>

      <md-app-content>
        <SignUp />

      </md-app-content>
    </md-app>
  </div>
</template>

<script lang="ts">


import SignUp from "./SignUp.vue";

export default {
  name: 'Waterfall',
  components:{
    SignUp
  }
}
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

</style>

