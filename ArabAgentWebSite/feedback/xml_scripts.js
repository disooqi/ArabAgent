function create_XMLHttp_Object()
{
	
	//alert(dname);
	if (window.XMLHttpRequest)
	  {
		xhttp=new XMLHttpRequest();
	  }
	else
	  {
		//alert("ActiveXObject");
		xhttp=new ActiveXObject("Microsoft.XMLHTTP");
	  }
	//alert("XMLHttp Object Created");
	//document.write("XML document loaded into an XML DOM Object.");
	return xhttp;
}

function loadXMLString(txt)
{
	if (window.DOMParser)
	  {
	  parser=new DOMParser();
	  xmlDoc=parser.parseFromString(txt,"text/xml");
	  }
	else // Internet Explorer
	  {
	  xmlDoc=new ActiveXObject("Microsoft.XMLDOM");
	  xmlDoc.async="false";
	  xmlDoc.loadXML(txt);
	  }
	  	alert("XML string loaded into an XML DOM Object.");
	return xmlDoc;
}