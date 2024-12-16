#eShopMicroservices
A backend for a web application of eShop Global, an ecommerce company using Microservices Architecture. Following operations can be performed:  
1.	Admin can add/remove new products to inventory (CRUD operations on products in ProductAPI).
2.	Admin can add/remove product details like size/price/design (CRUD operations on products' details in ProductDetailsAPI).
3.	Product service can fetch detail about the product like size/price/design from ProductDetailsAPI.
4.	User can view all the products list.
5. 	Product stock is managed in InventoryAPI.
6.  	User can add items or remove items in their cart.
7. 	User can place an order using OrderAPI.


##Tools/Technolgies 
1.	.NET Core for writing microservices.
2.	Eureka for Service Discovery
3.	Ocelot API Gateway for implementing routing
4.	Docker as deployment tool 
5.	Load balancer
6.	Message Communication pattern using RabbitMQ
7.	JWT authentication mechanism to secure APIs


##Setup Guide 
Docker should be installed.

### Run the Project
1.	Edit the eShopMicroservices_Up.bat in a text editor eg. Notepad.
2.	Put the correct project folder path under change directory command and save it.
3.	Do the same with the eShopMicroservices_Down.bat .
4. 	Now run the eShopMicroservices_Up.bat file.
5.	Use link http://localhost:8761/ to open interface for eureka server in a browser.
6.	Use link http://localhost:15672/ to open interface for rabbitmq server in a browser.
7. 	Open postaman and run the apis using baseurl: http://localhost:8001/
8.	To close the running containers run the eShopMicroservices_Down.bat file.

**URLs**
a)	ProductAPI
	http://localhost:8001/api/Product  (get and post)
	http://localhost:8001/api/Product/code  (get product by code)
	http://localhost:8001/api/Product/id (delete product)

b)	ProductDetailsAPI
	http://localhost:8001/api/ProductDetails  (get and post)
	http://localhost:8001/api/ProductDetails/code  (get product details by code)
	http://localhost:8001/api/ProductDetails/id (delete product details)

c)	UserAPI
	http://localhost:8001/api/auth/register (post - for registering a user) http://localhost:8001/api/auth/login     (post - for logging a user)
	http://localhost:8001/api/auth/assign-role (post - for assigning role to a registered user) 

d)	OrderAPI 
	http://localhost:8001/api/Order  (get and post)
	http://localhost:8001/api/Order/id  (get by id and delete)

e)	InventoryAPI 
	http://localhost:8001/api/Inventory  (get and post)
	http://localhost:8001/api/Inventory/id  (get by id and delete)

f)	CartAPI 
	http://localhost:8001/api/Order  (get and post)
	http://localhost:8001/api/Order/id  (get by id and delete)

g)	APIGateway
	http://localhost:8001


# Services Description

##UserAPI
UserAPI is build to manage the User authentication functionality. This service allows a user to register himself to the application. After registration he can login to the application. On successful login this service generates a unique JWT token that includes the role of the user and it is further used to perform Role based operations to the application. 

##ProductAPI 
ProductAPI service is used to manage the CRUD operations on product like Adding a product, fetch all products or by id, Update and delete a product. Add, update and delete operations on product is role based. Only an Admin user can perform those operations.

##OrderAPI
OrderAPI service is used to perform the order functionality. A logged in user can order products. The user can also view his order-items. 

##InventoryAPI
InventoryAPI service is used to manage the sock of product. An admin can perform CRUD operations on it. It is updated if a user places an order. 

##CartAPI
A user can use cart to manage items that they want to order. This cart service is provided by CartAPI service.

##ProductDetailsAPI
ProductDetailsAPI contains details and specifications for products. This service provides an admin with the functionality to manage details for products separately from the products itself.
##APIGateway
APIGateway service is used to provide a common Gateway Interface for all the services. It uses Ocelot for implementation in background. All the services end point can be accessed through it. 

Note:- In this project I have used JWT for Authentication and Microsoft Identity Service for Role based Authentication. Deployed all the services as docker images on docker Hub.


##Docker Images URL links of DockerHub 
a)	ProductAPI
https://hub.docker.com/repository/docker/sumitsaurabhss/productapi

b)	ProductDetailsAPI
https://hub.docker.com/repository/docker/sumitsaurabhss/productdetailsapi

c)	UserAPI 
https://hub.docker.com/repository/docker/sumitsaurabhss/userapi

d)	APIGateway 
https://hub.docker.com/repository/docker/sumitsaurabhss/apigateway

e)	OrderAPI 
https://hub.docker.com/repository/docker/sumitsaurabhss/orderapi

f)	InventoryAPI
https://hub.docker.com/repository/docker/sumitsaurabhss/inventoryapi

g)	CartAPI
https://hub.docker.com/repository/docker/sumitsaurabhss/cartapi