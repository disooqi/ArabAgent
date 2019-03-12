function xSend_Feedback()
{

var bool_var = true;
var counter = 0;

    while ((both_radio = document.getElementsByName('relevancy'+counter++)).length)
    {
        if(both_radio[0].checked == true)
            alert("Link No."+(counter-1) +", is Relevant");
        else if(both_radio[1].checked == true)
            alert("Link No."+(counter-1) +", is Irrelevant");
        else
            alert("Link No."+(counter-1) +" has not been evaluated yet!");
    }
}

function send_feedback(counter, rel_irr)
{
	document.getElementById('fb_ctl'+counter).disabled=true;
	
	if(document.getElementById('rs'+counter).innerHTML.indexOf('<H3') > -1) 
    {
		var result_string = document.getElementById('rs'+counter).innerHTML;
		var start_index = document.getElementById('rs'+counter).innerHTML.indexOf('href=\"')+6;
		var end_index = document.getElementById('rs'+counter).innerHTML.indexOf('\"', start_index);
		
		var XMLHttp_url = "search.aspx?fb_url="+result_string.substr(start_index, end_index - start_index)+"&fb_rel="+rel_irr;
		XMLHttp_obj = create_XMLHttp_Object();
		
		
		XMLHttp_obj.onreadystatechange=function()
        {
		//alert("Ya Rab 1");
            if (XMLHttp_obj.readyState==4 && XMLHttp_obj.status==200)
            {
				//("Ya Rab 2");
                document.getElementById('fb_ctl'+counter).disabled=false;
                document.getElementById('fb_ctl'+counter).innerHTML = "<img src=\"http://mail.google.com/mail/help/images/check.gif\">&nbsp;&nbsp;&nbsp;Thank you for your feedback.";
                document.getElementById('fb_ctl'+counter).style.color = "#43845a";
                //alert(XMLHttp_obj.responseText);
                //document.getElementById("myDiv").innerHTML=xmlhttp.responseText;
            }
        }
		XMLHttp_obj.open("GET", XMLHttp_url, true);
        XMLHttp_obj.send();
	}
	
}

function ysend_feedback(counter, rel_irr)
{    
	//alert("OK");
    //document.getElementById('fb_ctl'+counter).disabled=true;
	//alert("OK");
    //document.getElementById('thank'+counter).style.visibility="visible";
    
    if(document.getElementById('rs'+counter).innerHTML.indexOf('<H3') > -1) 
    {
	alert("OK3");
        var result_string = document.getElementById('rs'+counter).innerHTML;
        var start_index = document.getElementById('rs'+counter).innerHTML.indexOf('href=\"')+6;
        var end_index = document.getElementById('rs'+counter).innerHTML.indexOf('\"', start_index);
        
        var XMLHttp_url = "search.aspx?fb_url="+result_string.substr(start_index, end_index - start_index)+"&fb_rel="+rel_irr;
        
        
        
        XMLHttp_obj = create_XMLHttp_Object();
        //alert(XMLHttp_url);
        
        
        XMLHttp_obj.onreadystatechange=function()
        {
            if (XMLHttp_obj.readyState==4 && XMLHttp_obj.status==200)
            {
                document.getElementById('fb_ctl'+counter).disabled=false;
                document.getElementById('fb_ctl'+counter).innerHTML = "<img src=\"http://mail.google.com/mail/help/images/check.gif\">&nbsp;&nbsp;&nbsp;Thank you for your feedback.";
                document.getElementById('fb_ctl'+counter).style.color = "#43845a";
                alert(XMLHttp_obj.responseText);
                //document.getElementById("myDiv").innerHTML=xmlhttp.responseText;
            }
        }
        
        XMLHttp_obj.open("GET", XMLHttp_url, true);
        XMLHttp_obj.send();
    }
    
    //alert('Al7amdo lelah');
}


function hideAndShow(ElemID){
//var html_str = ;
if(document.getElementById(ElemID).innerHTML.indexOf('<img')==-1)
{
  //document.getElementById(ElemID).
  document.getElementById(ElemID).innerHTML +='<img src="http://mail.google.com/mail/help/images/check.gif">Thank you for your feedback.';
  
}
}

/*function showstuff(counter){
//alert('showstuff');
    document.getElementById('fb_ctl'+counter).style.color = "#43845a";
    document.getElementById('thank'+counter).style.visibility="visible";
}*/

//display:none;
function hidestuff(boxid){
   document.getElementById(boxid).style.visibility="hidden";
   //showstuff(getSpanID(document.getElementById('thanks1')));
   showstuff(getSpanID(boxid));
}


function message()
{
alert.description = "dsfs";//('message','title')
alert('Thanks! Your feedback will help us improve your future search and browsing activities.')

var radioButtons = document.getElementsByName('meta');   
//alert(radioButtons.length);
}



