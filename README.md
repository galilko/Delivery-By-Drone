Mini project in the Windows system (.net core) - system for managing shipments by skimmers - DDC (Drone Delivery Company):
Directed by Mr. Eliezer Ginsburger.
Presented by Gal Gabay and Yehuda Miletzky.

To log in as an administrator, type username "admin" and password "123".

Everything is realized according to the requirements of the project: 3-layers model (DAL [xml], BL, PL [wpf]).
Also realized a number of additions:
1. Implementation of the layered model with its extended configuration - structure 2. as well as the extended factory design templates and singelton.
2. Use of Data template.
3. Using Style
4. Deletion of entities - Some entities contain an active field and during deletion is moved to false. Deleted entities can be seen on the various screens with the help of an archive button. And of course inactive objects are not included in the residence.
5. Presenting coordinates in a sexadecimal way.
6. Use of converters as well as multiconverter
7. Design based on Google's design language - material design.
8. All the buttons are round and designed with the perception of icons from material design - a modern design that makes the user experience easier.
9. Inquiries to BL will be in BackgroundWorker
10. Customer interface
