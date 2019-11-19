var examplePosts = [{ postid: 1, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    {postid: 2, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 3, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."}];
var compsciPosts = [{ postid: 1, url:'#',subject: "LoremIpsum", postContent: "Foobar" },
    {postid: 2, url:'#',subject: "IpsumLorem", postContent: "Else{ Heart.Break() }"},
    {postid: 3, url:'#',subject: "LoremIpsum", postContent: "this.post = 'a post'"},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "this.post = 'another post'"},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Exam avg: 70%"},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."}];


var exampleCommunities = [{name: "Anthropology",url: "www.google.com"},{name: "Computer Science B.S.", url: "#"}, {name: "Mechanical Engineering", url: "#"}];

Vue.component('testpost', {
  template: '<p>This is my post with title</p>'
  /*methods: {
    updateTitle()
    {
      this.post_title = "Title" + (this.counter)+2;
    }
  }*/
});

Vue.component('postlistcomp', {
  template: `
   <ul  class="list-group list-group-flush w-70 float-md-left">
   <li class="list-group-item mb-3"  v-for="post in posts">
       <h4> {{ post.subject }} </h4>
       <p> {{ post.postContent }} </p>
   </li>
   </ul>`,
  data: function() {
    return {
      posts: examplePosts
    }
  }
});

var communityDropdown= new Vue({
    el: "#communityDrop",
    data: {
        communities: []
    },
    methods: {
        getCommunities(){
            this.communities = exampleCommunities;
            console.log(this.communities);
        }
    },
    beforeMount(){
        this.getCommunities()
    }
});

var postList = new Vue({
    el: "#post-list",
    data: {
        posts: []
    },
    methods: {
        getPosts(){
            this.posts = examplePosts;
            console.log(this.posts);
        }
    },
    beforeMount()
    {
        this.getPosts()
    }
});
