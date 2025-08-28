document.addEventListener('DOMContentLoaded', () => { 
  const className = 'reel-el'; 
  const reels = Array.from(document.querySelectorAll(`.${className}`)); 
  const toggleOverflowClass = elem => { 
	elem.classList.toggle('overflowing', elem.scrollWidth > elem.clientWidth); 
  }; 

  for (let reel of reels) { 
	if ('ResizeObserver' in window) { 
	  new ResizeObserver(entries => { 
		for (let entry of entries) { 
		  toggleOverflowClass(entry.target); 
		} 
	  }).observe(reel); 
	} 

	if ('MutationObserver' in window) { 
	  new MutationObserver(entries => { 
		for (let entry of entries) { 
		  toggleOverflowClass(entry.target); 
		} 
	  }).observe(reel, {childList: true}); 
	} 
  } 
});