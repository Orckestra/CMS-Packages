This folder was created by installing the StaticFileLinkMediaProvider CMS Package.

By default the package will give you access to link to files in this ~/download folder, but you can change this:

Open the file ~/App_Data/Composite/Composite.config and locate this element (search for StaticFileLinkMediaProvider):

      <add basePath="~/download" storeTitle="Static files from ~/download" storeDescription="" storeId="download" 
	       name="StaticFileLinkMediaProvider" type="Orckestra.Media.StaticFileLinkMediaProvider.MediaProvider, Orckestra.Media.StaticFileLinkMediaProvider" />

Change the attributes "basePath" and "storeTitle" to reflect your needs.

You can add more of this element, if you want to expose multiple isolated folders. In this case, you should also update
the attributes "storeId" and "name".

This file can be safely deleted. You many also safely delete this folder, if you change the basePath as described above.
