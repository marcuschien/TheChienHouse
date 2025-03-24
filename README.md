# Backend Engineering Practical Test

You will be implementing a simple API. You may use any language or technology you prefer. 
It's recommended that you set up a working development environment ahead of time.

You may ask questions, reference documentation, or search for help on Google, Stack Overflow, or anywhere else. 
Please email the person who set up your interview with any questions you have. 

When you have finished a stage in the interview, please commit you code so we can review each stage independently.

We ask that you take 4 hours to take the test. Don’t worry if you don’t complete every stage, we are more interested in how you approach the problem and where you choose to spend your time.

> [!TIP]
> We would rather see a less complete, but more polished submission than a complete but unpolished submission, so we can see an example of your best work.

Please be ready to share your screen with your service running to demonstrate your API endpoints to us (using the tool of your choice, e.g. Postman, cURL, etc). We may ask you to make some requests to test your API so it would be handy to have this prepared beforehand.

## Stage 1

`GET /products`
Create an API endpoint that serves a list of products that can be sold.

A product consists of:
- A unique ID
- A name
- A price

Products may be stored in memory. An external database may be used, but is not required.

|      Name      | Price  |
|:--------------:|:------:|
| Chrome Toaster |  $100  |
| Copper Kettle  | $49.99 |
|  Mixing Bowl   |  $20   |


## Stage 2
`POST /products` Create an API endpoint that allows creating a new product.

The response should contain the product details in the same format as Stage 1.

The new product must be persisted so that the endpoint from stage 1 includes the new product in its responses.

## Stage 3
`POST /sales` Create an API endpoint that allows sales to be made. It is not necessary to persist sales.

A sale request consists of:
- An array of line items.
- Each line item includes a product ID and a quantity.

A sale response consists of:
- Everything in the sale request
- A total for each line item.
- A total price of sale.

## Stage 4
Modify the sales API to allow discounts on the overall sale. The discount on the sale is a flat dollar amount (i.e. $10 off the total as opposed to 10% off the total).

The sale request is modified to add a new discount field, representing the total discount across the entire sale.

For tax reasons, the discount must be spread proportionally across the line items in the sale, taking into account line item quantity and price. Each line item in the response should also include a discount field containing the proportion of the discount for that line item.

