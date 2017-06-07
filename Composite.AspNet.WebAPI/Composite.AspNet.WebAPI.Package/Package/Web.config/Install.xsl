<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="asm"
								xmlns:asm="urn:schemas-microsoft-com:asm.v1" >
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.web/compilation/assemblies">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@assembly='System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'])=0">
				<add assembly="System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.webServer/modules">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(remove[@name='WebDAVModule'])=0">
				<remove name="WebDAVModule" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="configuration">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />

			<xsl:if test="count(runtime/asm:assemblyBinding/asm:dependentAssembly/asm:assemblyIdentity[@name='System.Net.Http.Formatting']) = 0" xml:space="preserve">
				<runtime>
					<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
						<dependentAssembly>
							<assemblyIdentity name="System.Net.Http.Formatting" culture="neutral" publicKeyToken="31bf3856ad364e35" />
							<bindingRedirect oldVersion="0.0.0.0-5.2.3.0" newVersion="5.2.3.0" />
						</dependentAssembly>
					</assemblyBinding>
			</runtime>
		</xsl:if>

			<xsl:if test="count(runtime/asm:assemblyBinding/asm:dependentAssembly/asm:assemblyIdentity[@name='System.Web.Http']) = 0" xml:space="preserve">
				<runtime>
					<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
						<dependentAssembly>
							<assemblyIdentity name="System.Web.Http" culture="neutral" publicKeyToken="31bf3856ad364e35" />
							<bindingRedirect oldVersion="0.0.0.0-5.2.3.0" newVersion="5.2.3.0" />
						</dependentAssembly>
					</assemblyBinding>
				</runtime>
		</xsl:if>

		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>