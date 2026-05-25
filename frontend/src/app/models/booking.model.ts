export interface BookingRequest {
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  promoCode?: string;
  redeemPoints?: number;
  paymentMethod: string;
  guests?: number;
  roomsCount?: number;
}

export interface BookingResponse {
  id: number;
  userId: number;
  username: string;
  roomId: number;
  roomType: string;
  hotelId: number;
  hotelName: string;
  hotelLocation: string;
  checkInDate: string;
  checkOutDate: string;
  totalPrice: number;
  reservationNumber: string;
  status: string;
  roomsCount: number;
  createdAt: string;
}

export interface Promotion {
  id?: number;
  code: string;
  discountType: string; // Flat, Percentage
  discountValue: number;
  active: boolean;
  expiryDate: string;
}
