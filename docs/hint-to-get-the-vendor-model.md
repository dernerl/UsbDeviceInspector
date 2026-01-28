https://www.reddit.com/r/crowdstrike/comments/10zwusm/combined_id_vendor_model/

So I have been able to get the USB Serial Number quite easily from both Powershell and Python, I am however having no luck trying to get the Vendor / Model to basically create the combined_id. I am unable to find it anywhere in Windows or via libusb on Python.

My end goal is to create a script to integrate with out service desk so that when a request for a USB exclusion comes in, our team can run a script on the end users machine which will output a combined_id or multiple if they have multiple usb mass storage devices plugged in. but they will tell us the make (Kingston Datatraveler) as an example. Then the the team can just click approved which will be added to the policy.

I have got everything working now apart from creating the combined ID, For example, my samsung drive has 2316_4096_<serial> but 2316 or 4096 I am unable to find anywhere when the device is plugged in. only CS can see it. In Windows Device Manager the VID shows up as a different id.
