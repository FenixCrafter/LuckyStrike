<template>
  <div class="loader-container">
    <div class="loader">
      <div class="ball-gradient"></div>
      <div class="ball">
        <div class="hole"></div>
        <div class="hole"></div>
        <div class="hole"></div>
      </div>
    </div>
    <div class="shadow"></div>
    <div class="loader-text">
      LADEN...
    </div>
  </div>
</template>

<style scoped>
.loader-container {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(25, 25, 25, .2);
  text-align: center;
  padding-top: 100px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding-bottom: 80%;
}
.loader {
  position: relative;
  width: 100px;
  height: 100px;
  -moz-border-radius: 50px;
  -webkit-border-radius: 50px;
  border-radius: 50px;
  overflow: hidden;
  margin: 0 auto;
}
.ball {
  position: absolute;
  top: 0;
  left: 0;
  width: 100px;
  height: 100px;
  -moz-border-radius: 50px;
  -webkit-border-radius: 50px;
  border-radius: 50px;
  z-index: 50;
  -webkit-animation: spin .75s infinite;
  -ms-animation: spin .75s infinite;
  -moz-animation: spin .75s infinite;
  animation: spin .75s infinite;
}
.ball-gradient {
  position: absolute;
  top: -70px;
  left: -60px;
  width: 200px;
  height: 200px;
  background-position: -50px -50px;
  background: #d8c87f; /* Old browsers */
  /* IE9 SVG, needs conditional override of 'filter' to 'none' */
  background: url(data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiA/Pgo8c3ZnIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgdmlld0JveD0iMCAwIDEgMSIgcHJlc2VydmVBc3BlY3RSYXRpbz0ibm9uZSI+CiAgPHJhZGlhbEdyYWRpZW50IGlkPSJncmFkLXVjZ2ctZ2VuZXJhdGVkIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgY3g9IjUwJSIgY3k9IjUwJSIgcj0iNzUlIj4KICAgIDxzdG9wIG9mZnNldD0iMTElIiBzdG9wLWNvbG9yPSIjZDhjODdmIiBzdG9wLW9wYWNpdHk9IjEiLz4KICAgIDxzdG9wIG9mZnNldD0iMTElIiBzdG9wLWNvbG9yPSIjZDhjODdmIiBzdG9wLW9wYWNpdHk9IjEiLz4KICAgIDxzdG9wIG9mZnNldD0iMTIlIiBzdG9wLWNvbG9yPSIjZGRiYjIyIiBzdG9wLW9wYWNpdHk9IjEiLz4KICAgIDxzdG9wIG9mZnNldD0iMzQlIiBzdG9wLWNvbG9yPSIjZGRiYjIyIiBzdG9wLW9wYWNpdHk9IjEiLz4KICAgIDxzdG9wIG9mZnNldD0iMzUlIiBzdG9wLWNvbG9yPSIjYTM4NTAwIiBzdG9wLW9wYWNpdHk9IjEiLz4KICAgIDxzdG9wIG9mZnNldD0iMTAwJSIgc3RvcC1jb2xvcj0iI2EzODUwMCIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgPC9yYWRpYWxHcmFkaWVudD4KICA8cmVjdCB4PSItNTAiIHk9Ii01MCIgd2lkdGg9IjEwMSIgaGVpZ2h0PSIxMDEiIGZpbGw9InVybCgjZ3JhZC11Y2dnLWdlbmVyYXRlZCkiIC8+Cjwvc3ZnPg==);
  background: -moz-radial-gradient(center, ellipse cover, #d8c87f 11%, #d8c87f 11%, #ddbb22 12%, #ddbb22 34%, #a38500 35%, #a38500 100%); /* FF3.6+ */
  background: -webkit-gradient(radial, center center, 0px, center center, 100%, color-stop(11%,#d8c87f), color-stop(11%,#d8c87f), color-stop(12%,#ddbb22), color-stop(34%,#ddbb22), color-stop(35%,#a38500), color-stop(100%,#a38500)); /* Chrome,Safari4+ */
  background: -webkit-radial-gradient(center, ellipse cover, #d8c87f 11%,#d8c87f 11%,#ddbb22 12%,#ddbb22 34%,#a38500 35%,#a38500 100%); /* Chrome10+,Safari5.1+ */
  background: -o-radial-gradient(center, ellipse cover, #d8c87f 11%,#d8c87f 11%,#ddbb22 12%,#ddbb22 34%,#a38500 35%,#a38500 100%); /* Opera 12+ */
  background: -ms-radial-gradient(center, ellipse cover, #d8c87f 11%,#d8c87f 11%,#ddbb22 12%,#ddbb22 34%,#a38500 35%,#a38500 100%); /* IE10+ */
  background: radial-gradient(ellipse at center, #d8c87f 11%,#d8c87f 11%,#ddbb22 12%,#ddbb22 34%,#a38500 35%,#a38500 100%); /* W3C */
  filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#d8c87f', endColorstr='#a38500',GradientType=1 ); /* IE6-8 fallback on horizontal gradient */
  z-index: 31;
}
.hole:nth-child(2) {
  position: absolute;
  top: 20px;
  left: 35px;
  width: 10px;
  height: 10px;
  background: rgba(0,0,0,.6);
  -moz-border-radius: 5px;
  -webkit-border-radius: 5px;
  border-radius: 5px;
  z-index: 55;
}
.hole:nth-child(3) {
  position: absolute;
  top: 38px;
  left: 50px;
  width: 10px;
  height: 10px;
  background: rgba(0,0,0,.6);
  -moz-border-radius: 5px;
  -webkit-border-radius: 5px;
  border-radius: 5px;
  z-index: 55;
}
.hole:nth-child(1) {
  position: absolute;
  top: 18px;
  left: 60px;
  width: 10px;
  height: 10px;
  background: rgba(0,0,0,.6);
  -moz-border-radius: 5px;
  -webkit-border-radius: 5px;
  border-radius: 5px;
  z-index: 55;
}
.shadow {
  width: 100px;
  height: 25px;
  background: rgba(0,0,0,.6);
  -moz-border-radius: 100px / 25px;
  -webkit-border-radius: 100px / 25px;
  border-radius: 100px / 25px;
  margin: -14px auto 0;
}
@-webkit-keyframes spin {
  from { -webkit-transform: rotate(0deg); }
  to { -webkit-transform: rotate(360deg); }
}
@-ms-keyframes spin {
  from { -ms-transform: rotate(0deg); }
  to { -ms-transform: rotate(360deg); }
}
@-moz-keyframes spin {
  from { -moz-transform: rotate(0deg); }
  to { -moz-transform: rotate(360deg); }
}
@keyframes spin {
  from {transform:rotate(0deg);}
  to {transform:rotate(360deg);}
}
.loader-text {
  color: black;
  font-size: 14px;
  text-transform: uppercase;
  margin-top: 15px;
  font-family: 'Ariel', sans-serif;
  margin-left: 14px;
  font-weight: bold;
  margin-bottom: 20%;
}
</style>