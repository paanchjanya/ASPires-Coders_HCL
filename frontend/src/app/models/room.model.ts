export interface Amenity {
  id: number;
  name: string;
  icon: string;
}

export interface Room {
  id: number;
  hotelId: number;
  hotelName?: string;
  roomType: string;
  pricePerNight: number;
  capacity: number;
  description: string;
  imageUrl: string;
  status: string;
  amenities?: Amenity[];
}
