import React from 'react';
import { Box, Flex, Heading, Text } from '@chakra-ui/react';
import { Link } from 'react-router-dom';
import formatCurrency from '../utils/formatCurrency';

const CartItems = ({ cart }) => {
  return (
    <Box>
      {cart.items.map((item) => (
        <Flex key={item.product.id}>
          <Heading
            size='md'
            my='3'
            as={Link}
            to={`/products/${item.product.id}`}
          >
            {renderArtistNames(item.product.artists)} &mdash;{' '}
            {item.product.title}
          </Heading>
          <Text ms='auto'>
            {item.quantity} x {formatCurrency(item.product.price)}
          </Text>
        </Flex>
      ))}
      <Flex>
        <Text>Загальна ціна:</Text>
        <Text ms='auto' fontSize='2xl'>
          {formatCurrency(cart.totalPrice)}
        </Text>
      </Flex>
    </Box>
  );
};

const renderArtistNames = (artists) => {
  return artists.map((artist, index) => {
    if (index === 0) {
      return artist.name;
    } else {
      return `, ${artist.name}`;
    }
  });
};

export default CartItems;
