<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<%@ page import="com.google.appengine.api.users.User" %>
<%@ page import="com.google.appengine.api.users.UserService" %>
<%@ page import="com.google.appengine.api.users.UserServiceFactory" %>

<html>
<head>
<title>ArabAgent! Your Assistant through Arabic Web</title>
</head>
  <body>

<%
    UserService userService = UserServiceFactory.getUserService();
    User user = userService.getCurrentUser();
    if (user != null) {
%>
<p>Hello, <%= user.getNickname() %>! (You can
<a href="<%= userService.createLogoutURL(request.getRequestURI()) %>">sign out</a>.)</p>

  <form action="/arabagent" method="post">
  <center>
  <div><p> Paste an Arabic Article in the box below </p></div>
    <div><textarea name="content" rows="35" cols="60"></textarea></div>
    <div><input type="submit" name="sim" value="Test Similarity with you Profile" />
    <input type="submit" name="update" value="Update you Profile" />
    </div>
    </center>
  </form>
<%
    } else {
%>
<p>Hello!
<a href="<%= userService.createLoginURL(request.getRequestURI()) %>">Sign in</a>.</p>
<p>You need a google account to test ArabAgent system, or you could use the following testing account:</p>
<p><strong>Username:arabagent.test@gmail.com</strong></p>
<p><strong>Password:arabagent</strong></p>
<%
    }
%>
<p>This web page is intended for testing the under developing ArabAgent System. Although the system works with
full functionalities, some errors may object you due to it is in an early stage of testing. </p>
<p>for more information about ArabAgent System please refer to <a href="http://arabagent.wikispaces.com">this web site</a></p>


  </body>
</html>