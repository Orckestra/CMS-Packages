export const LOCALIZATION_DEFAULT_PROVIDER = 'Orckestra.ExperienceManagement.KeywordRedirect';

export function close() {
	return (dispatch, getState) => {
		if (top.Application) {
			let action = new top.Action({ bindingWindow: window }, top.PageBinding.ACTION_RESPONSE);
			top.UserInterface.getBinding(window.frameElement.parentNode).dispatchAction(action);
		}
	}
}

export function queryStringParam(name) {
	return unescape(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + escape(name).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}


export function getString(string) {
	let result = null;
	if (top.Application) {
		let provider = null;
		let key = string
		if (key.indexOf(":") > -1) {
			provider = key.split(":")[0];
			key = key.split(":")[1];
		}
		else {
			provider = LOCALIZATION_DEFAULT_PROVIDER;
		}
		result = top.StringBundle.getString(provider, key);
	}
	result = result ? result : "(?)";
	for (let i = 1; i < arguments.length; i++) {
		result = result.replace("{" + (i - 1) + "}", arguments[i]);
	}
	return result;
}

export function dialogError(title, string) {
	return (dispatch, getState) => {
		return new Promise(function (resolve, reject) {
			if (top.Application) {
				let action = new top.Action({ bindingWindow: window }, top.PageBinding.ACTION_RESPONSE);
				top.Dialog.error(title, string, null,
					{
						handleDialogResponse: () => { resolve(); }
					}
				);
			} else {
				alert(title + '\n' + string);
				reject();
			}
		});
	}
}


export const PermissionType =
	{
		Read: 0,
		Edit: 1,
		Add: 2,
		Delete: 3,
		Approve: 4,
		Publish: 5,
		Administrate: 6,
		ClearPermissions: 7,
		Configure: 8
	};

