# Endpoints

## Users and Auth
- `/users/login` (POST)
  - Request Body:
  ```json
  {
    "username": "string",
    "password": "string",
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "Login successful.",
    "data": {
      "id": "string",
      "username": "string",
      "role": "string",
      "token": "string"
    }
  }
  ```
- `/users/register` (POST)
  - Request Body:
  ```json
  {
    "username": "string",
    "fullName": "string",
    "email": "string",
    "phone": "string",
    "password": "string",
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "Register successful.",
    "data": null
  }
  ```
- `/users/me` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Profile fetched successfully",
    "data": {
      "id": 0,
      "username": "string",
      "fullName": "string",
      "email": "string",
      "phone": "string",
      "balance": 1000,
      "environmentalImpact": 1000,
    }
  }
  ```
- `/users/points` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Points fetched successfully",
    "data": {
      "username": "string",
      "totalPoints": 10000,
      "currentBalance": 2000,
      "environmentalImpact": 2000,
      "redeemedPoints": 8000,
    }
  }
  ```
- `/users` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Users fetched successfully",
    "data": [
      {
        "id": 0,
        "username": "string",
        "fullName": "string",
        "email": "string",
        "phone": "string",
      },
      {
        "id": 1,
        "username": "string",
        "fullName": "string",
        "email": "string",
        "phone": "string",
      }
    ]
  }
  ```
- `/users` (POST)
  - Request Body:
  ```json
  {
    "username": "string",
    "fullName": "string",
    "email": "string",
    "phone": "string",
    "password": "string",
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "Users created successfully",
    "data": null
  }
  ```
- `/users/{id}` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Users fetched successfully",
    "data": {
      "id": 0,
      "username": "string",
      "fullName": "string",
      "email": "string",
      "phone": "string",
    }
  }
  ```
- `/users/{id}` (PUT)
  - Request Body:
  ```json
  {
    "username": "string",
    "fullName": "string",
    "email": "string",
    "phone": "string",
    "password": "string",
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "Users updated successfully",
    "data": null
  }
  ```
- `/users/{id}` (DELETE)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Users removed successfully",
    "data": null
  }
  ```


## Leaderboard
- `/leaderboard` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Leaderboard fetched successfully",
    "data": [
      {
        "rank": 1,
        "fullName": "string",
        "totalPoints": 1000,
        "currentBalance": 1000,
        "environmentalImpact": 1000,
      },
      {
        "rank": 2,
        "fullName": "string",
        "totalPoints": 1000,
        "currentBalance": 1000,
        "environmentalImpact": 1000,
      },
    ]
  }
  ```
- `/leaderboard/my-rank` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Leaderboard fetched successfully",
    "data": {
      "rank": 100,
      "fullName": "string",
      "totalPoints": 1000,
      "environmentalImpact": 1000,
    }
  }
  ```

## WasteType
- `/waste-type` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "WasteType fetched successfully",
    "data": [
      {
        "id": 0,
        "name": "string",
        "code": "string",
        "pointTariff": 100,
        "CO2Factor": 20,
        "isActive": true,
      },
      {
        "id": 1,
        "name": "string",
        "code": "string",
        "pointTariff": 100,
        "CO2Factor": 20,
        "isActive": true,
      }
    ]
  }
  ```
- `/waste-type` (POST)
  - Request Body:
  ```json
  {
    "name": "string",
    "code": "string",
    "pointTariff": 100,
    "CO2Factor": 20,
    "isActive": true,
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "WasteType created successfully",
    "data": null
  }
  ```
- `/waste-type/{id}` (GET)
  - Request Body(empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "WasteType fetched successfully",
    "data": {
      "id": 0,
      "name": "string",
      "code": "string",
      "pointTariff": 100,
      "CO2Factor": 20,
      "isActive": true,
    }
  }
  ```
- `/waste-type/{id}` (PUT)
  - Request Body:
  ```json
  {
    "name": "string",
    "code": "string",
    "pointTariff": 100,
    "CO2Factor": 20,
    "isActive": true,
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "WasteType updated successfully",
    "data": null
  }
  ```
- `/waste-type/{id}` (DELETE)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "WasteType removed successfully",
    "data": null
  }
  ```
- `/waste-type/top` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "WasteType reports fetched successfully",
    "data": [
      {
        "id": 0,
        "name": "string",
        "totalWeight": 10000,
        "totalPoints": 500000,
      },
      {
        "id": 1,
        "name": "string",
        "totalWeight": 10000,
        "totalPoints": 500000,
      }
    ]
  }
  ```

## Vouchers
- `/vouchers` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Vouchers fetched successfully",
    "data": [
      {
        "id": 0,
        "name": "string",
        "code": "string",
        "pointCost": 100,
        "isActive": true,
      },
      {
        "id": 1,
        "name": "string",
        "code": "string",
        "pointCost": 100,
        "isActive": true,
      }
    ]
  }
  ```
- `/vouchers` (POST)
  - Request Body:
  ```json
  {
    "name": "string",
    "code": "string",
    "pointCost": 100,
    "isActive": true,
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher created successfully",
    "data": null
  }
  ```
- `/vouchers/{id}` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher created successfully",
    "data": {
      "id": 0,
      "name": "string",
      "code": "string",
      "pointCost": 100,
      "isActive": true,
    }
  }
  ```
- `/vouchers/{id}` (PUT)
  - Request Body:
  ```json
  {
    "name": "string",
    "code": "string",
    "pointCost": 100,
    "isActive": true,
  }
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher updated successfully",
    "data": null
  }
  ```
- `/vouchers/{id}` (DELETE)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher removed successfully",
    "data": null
  }
  ```
- `/vouchers/top` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher Reports fetched successfully",
    "data": [
      {
        "id": 0,
        "name": "string",
        "totalRedemption": 100,
        "totalPointsRedeemed": 100000,
        "isActive": true,
      },
      {
        "id": 1,
        "name": "string",
        "totalRedemption": 100,
        "totalPointsRedeemed": 100000,
        "isActive": true,
      }
    ]
  }
  ```
- `/vouchers/{code}/detail` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher fetched successfully",
    "data": {
      "id": 0,
      "voucher": {
        "id": 0,
        "name": "string",
        "code": "string",
        "pointCost": 100,
        "isActive": true
      },
      "resident": {
        "id": 0,
        "fullName": "string",
        "email": "string"
      },
      "code": "string",
      "amount": 100,
      "isUsed": false,
      "createdAt": "2026-01-01T00:00:00Z",
      "updatedAt": "2026-01-01T00:00:00Z"
    }
  }
  ```
- `/vouchers/{id}/redeem` (POST)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher redeemed successfully",
    "data": null
  }
  ```
- `/vouchers/{id}/exchange` (POST)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Voucher exchanged successfully",
    "data": null
  }
  ```

## Deposits
- `/deposits` (GET)
  - Request Body (empty):
  ```json
  ```
  - Response Body (success):
  ```json
  {
    "message": "Deposits fetched successfully",
    "data": [
      {
        "id": 0,
        "resident": {
          "id": 0,
          "fullName": "string",
          "email": "string",
        },
        "wasteType": {
          "id": 0,
          "name": "string",
          "code": "string",
          "pointTariff": 100,
          "isActive": true
        },
        "estimatedWeight": 1.5,
        "actualWeight": null | 1.5,
        "estimatedPoints": 100,
        "actualPoints": 100,
        "status": "string",
      },
      {
        "id": 1,
        "resident": {
          "id": 0,
          "fullName": "string",
          "email": "string",
        },
        "wasteType": {
          "id": 0,
          "name": "string",
          "code": "string",
          "pointTariff": 100,
          "isActive": true
        },
        "estimatedWeight": 1.5,
        "actualWeight": null | 1.5,
        "estimatedPoints": 100,
        "actualPoints": 100,
        "status": "string",
      }
    ]
  }
  ```
- `/deposits` (POST)
  - Request Body (multipart/form-data):
  ```txt
    wasteTypeId = 0,
    estimatedWeight = 1.5,
    notes = "string",
    photo = ... (file)
  ```
  - Response Body (success):
  ```json
  {
    "message": "Deposit submitted successfully",
    "data": null
  }
  ```
- `/deposits/{id}` (GET)
  - Request Body (empty):
  ```
  ```
  - Response Body (success):
  ```json
  {
    "message": "Deposit fetched successfully",
    "data": {
      "id": 1,
      "resident": {
        "id": 0,
        "fullName": "string",
        "email": "string",
      },
      "wasteType": {
        "id": 0,
        "name": "string",
        "code": "string",
        "pointTariff": 100,
        "isActive": true
      },
      "estimatedWeight": 1.5,
      "estimatedPoints": 100,
      "actualWeight": null | 1.5,
      "actualPoints": null | 100,
      "status": "string",
      "notes": "string",
      "rejectionReason": "string",
      "photoPath": "string",
      "createdAt": "2026-01-01T00:00:00Z",
      "updatedAt": "2026-01-01T00:00:00Z",
    }
  }
  ```
- `/deposits/{id}` (PUT)
  - Request Body (multipart/form-data):
  ```txt
    wasteTypeId = 0,
    estimatedWeight = 1.5,
    notes = "string",
    photo = ... (file)
  ```
  - Response Body (success):
  ```json
  {
    "message": "Deposit updated successfully",
    "data": null
  }
  ```
- `/deposits/{id}/verify` (PUT)
  - Request Body (multipart/form-data):
  ```txt
    wasteTypeId = 0,
    actualWeight = 1.5,
    rejectionReason = "string",
    verified = true,
    photo = ... (file)
  ```
  - Response Body (success):
  ```json
  {
    "message": "Deposit updated successfully",
    "data": null
  }
  ```

